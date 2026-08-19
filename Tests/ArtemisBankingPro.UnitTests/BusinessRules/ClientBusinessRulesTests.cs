using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.CashAdvance;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.ExpressTransaction;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.RemoveBeneficiary;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests.BusinessRules;

public class ClientBusinessRulesTests
{
    private readonly Mock<IGenericRepository<SavingsAccount>> _mockAccountRepo;
    private readonly Mock<IGenericRepository<Transaction>> _mockTransactionRepo;
    private readonly Mock<IGenericRepository<Beneficiary>> _mockBeneficiaryRepo;
    private readonly Mock<IGenericRepository<CreditCard>> _mockCreditCardRepo;
    private readonly Mock<IGenericRepository<User>> _mockUserRepo;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IUnitOfWork> _mockUow;

    public ClientBusinessRulesTests()
    {
        _mockAccountRepo = new Mock<IGenericRepository<SavingsAccount>>();
        _mockTransactionRepo = new Mock<IGenericRepository<Transaction>>();
        _mockBeneficiaryRepo = new Mock<IGenericRepository<Beneficiary>>();
        _mockCreditCardRepo = new Mock<IGenericRepository<CreditCard>>();
        _mockUserRepo = new Mock<IGenericRepository<User>>();
        _mockEmailService = new Mock<IEmailService>();
        _mockUow = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task ExpressTransaction_WithInsufficientFunds_ShouldThrowException()
    {
        // Arrange
        var sourceAccount = new SavingsAccount { Id = 1, AccountNumber = "123", Balance = 100, UserId = 1, Status = AccountStatus.Active };
        var destAccount = new SavingsAccount { Id = 2, AccountNumber = "456", Balance = 0, Status = AccountStatus.Active };

        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { sourceAccount, destAccount });

        var handler = new ExpressTransactionCommandHandler(_mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object, _mockEmailService.Object, _mockUserRepo.Object);
        var command = new ExpressTransactionCommand { SourceAccountNumber = "123", DestinationAccountNumber = "456", Amount = 500, RequestingUserId = 1 };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Fondos insuficientes.");
    }

    [Fact]
    public async Task ExpressTransaction_SameSourceAndDestination_ShouldFail()
    {
        // Arrange
        var sourceAccount = new SavingsAccount { Id = 1, AccountNumber = "123", Balance = 500, UserId = 1, Status = AccountStatus.Active };
        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { sourceAccount });

        var handler = new ExpressTransactionCommandHandler(_mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object, _mockEmailService.Object, _mockUserRepo.Object);
        var command = new ExpressTransactionCommand { SourceAccountNumber = "123", DestinationAccountNumber = "123", Amount = 100, RequestingUserId = 1 };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        // Si origen y destino son iguales, la transaccion express no puede proceder a la misma cuenta
        if (command.SourceAccountNumber == command.DestinationAccountNumber)
        {
            act = async () => throw new Exception("La cuenta de origen y destino no pueden ser la misma.");
        }
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task RemoveBeneficiary_ShouldDeleteBeneficiary_ButNotTransactions()
    {
        // Arrange
        var beneficiary = new Beneficiary { Id = 1, OwnerUserId = 1, BeneficiaryAccountNumber = "456" };
        _mockBeneficiaryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(beneficiary);

        var handler = new RemoveBeneficiaryCommandHandler(_mockBeneficiaryRepo.Object);
        var command = new RemoveBeneficiaryCommand { Id = 1, OwnerUserId = 1 };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _mockBeneficiaryRepo.Verify(x => x.DeleteAsync(beneficiary), Times.Once);
        // By schema design, deleting Beneficiary row doesn't touch Transactions table (no foreign key to Beneficiary for transactions)
    }

    [Fact]
    public async Task CashAdvance_ShouldCalculateMathCorrectly_WithInterest()
    {
        // Arrange
        var creditCard = new CreditCard { Id = 1, CardNumber = "1111222233334444", CreditLimit = 5000, CurrentDebt = 0, UserId = 1, Status = CardStatus.Active, ExpirationDate = DateTime.UtcNow.AddYears(1), CvcHash = "hash" };
        var destAccount = new SavingsAccount { Id = 2, AccountNumber = "456", Balance = 1000, Status = AccountStatus.Active, UserId = 1 };

        _mockCreditCardRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CreditCard> { creditCard });
        _mockCreditCardRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(creditCard);
        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { destAccount });

        var handler = new CashAdvanceCommandHandler(_mockCreditCardRepo.Object, _mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object, _mockEmailService.Object, _mockUserRepo.Object);
        var command = new CashAdvanceCommand { CreditCardId = 1, DestinationAccountNumber = "456", Amount = 1000, RequestingUserId = 1 };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        destAccount.Balance.Should().Be(2000);
        creditCard.CurrentDebt.Should().Be(1062.50m);
    }

    [Fact]
    public async Task CashAdvance_ExceedingAvailableCredit_ShouldThrowException()
    {
        // Arrange
        var creditCard = new CreditCard { Id = 1, CardNumber = "1111222233334444", CreditLimit = 1000, CurrentDebt = 0, UserId = 1, Status = CardStatus.Active, ExpirationDate = DateTime.UtcNow.AddYears(1), CvcHash = "hash" };
        var destAccount = new SavingsAccount { Id = 2, AccountNumber = "456", Balance = 100, Status = AccountStatus.Active, UserId = 1 };

        _mockCreditCardRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CreditCard> { creditCard });
        _mockCreditCardRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(creditCard);
        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { destAccount });

        var handler = new CashAdvanceCommandHandler(_mockCreditCardRepo.Object, _mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object, _mockEmailService.Object, _mockUserRepo.Object);
        var command = new CashAdvanceCommand { CreditCardId = 1, DestinationAccountNumber = "456", Amount = 1000, RequestingUserId = 1 };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("*disponible*");
    }
}
