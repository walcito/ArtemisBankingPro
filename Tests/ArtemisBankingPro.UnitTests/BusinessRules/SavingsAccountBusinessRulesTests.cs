using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Deposit;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests.BusinessRules;

public class SavingsAccountBusinessRulesTests
{
    private readonly Mock<IGenericRepository<SavingsAccount>> _mockAccountRepo;
    private readonly Mock<IGenericRepository<Transaction>> _mockTransactionRepo;
    private readonly Mock<IGenericRepository<User>> _mockUserRepo;
    private readonly Mock<IUnitOfWork> _mockUow;

    public SavingsAccountBusinessRulesTests()
    {
        _mockAccountRepo = new Mock<IGenericRepository<SavingsAccount>>();
        _mockTransactionRepo = new Mock<IGenericRepository<Transaction>>();
        _mockUserRepo = new Mock<IGenericRepository<User>>();
        _mockUow = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task CreateAccount_ShouldGenerate_UniqueNineDigitAccountNumber()
    {
        // Arrange
        var userId = 1;
        _mockUserRepo.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(new User { Id = userId, IsActive = true });

        var existingAccounts = new List<SavingsAccount>
        {
            new SavingsAccount { AccountNumber = "123456789", UserId = userId, Type = AccountType.Principal, Status = AccountStatus.Active }
        };
        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(existingAccounts);

        SavingsAccount capturedAccount = null;
        _mockAccountRepo.Setup(x => x.AddAsync(It.IsAny<SavingsAccount>()))
            .Callback<SavingsAccount>(acc => capturedAccount = acc)
            .ReturnsAsync((SavingsAccount acc) => acc);

        var handler = new CreateSavingsAccountCommandHandler(_mockUserRepo.Object, _mockAccountRepo.Object, _mockTransactionRepo.Object);
        var command = new CreateSavingsAccountCommand { UserId = userId, InitialAmount = 500 };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        capturedAccount.Should().NotBeNull();
        capturedAccount.AccountNumber.Should().HaveLength(9);
        capturedAccount.AccountNumber.Should().MatchRegex("^[0-9]{9}$");
        capturedAccount.AccountNumber.Should().NotBe("123456789"); // Must be unique
    }

    [Fact]
    public async Task CancelPrimaryAccount_ShouldThrowException_UnderNoCircumstances()
    {
        // Arrange
        var existingAccounts = new List<SavingsAccount>
        {
            new SavingsAccount { AccountNumber = "111222333", Type = AccountType.Principal, Status = AccountStatus.Active }
        };
        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(existingAccounts);

        var handler = new CancelSavingsAccountCommandHandler(_mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object);
        var command = new CancelSavingsAccountCommand { AccountNumber = "111222333" };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Solo se pueden cancelar cuentas secundarias.");
    }

    [Fact]
    public async Task CancelSecondaryAccount_WithBalance_ShouldCreateTwoTransactions_AndTransferToPrimary()
    {
        // Arrange
        var userId = 1;
        var primaryAccount = new SavingsAccount { Id = 1, AccountNumber = "111111111", UserId = userId, Type = AccountType.Principal, Status = AccountStatus.Active, Balance = 1000 };
        var secondaryAccount = new SavingsAccount { Id = 2, AccountNumber = "222222222", UserId = userId, Type = AccountType.Secondary, Status = AccountStatus.Active, Balance = 500 };

        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { primaryAccount, secondaryAccount });

        var capturedTransactions = new List<Transaction>();
        _mockTransactionRepo.Setup(x => x.AddAsync(It.IsAny<Transaction>()))
            .Callback<Transaction>(t => capturedTransactions.Add(t))
            .ReturnsAsync((Transaction t) => t);

        var handler = new CancelSavingsAccountCommandHandler(_mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object);
        var command = new CancelSavingsAccountCommand { AccountNumber = "222222222" };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        capturedTransactions.Should().HaveCount(2);
        
        var debitTx = capturedTransactions.Single(t => t.Type == TransactionType.Debit);
        debitTx.SavingsAccountId.Should().Be(secondaryAccount.Id);
        debitTx.Amount.Should().Be(500);

        var creditTx = capturedTransactions.Single(t => t.Type == TransactionType.Credit);
        creditTx.SavingsAccountId.Should().Be(primaryAccount.Id);
        creditTx.Amount.Should().Be(500);

        primaryAccount.Balance.Should().Be(1500);
        secondaryAccount.Balance.Should().Be(0);
        secondaryAccount.Status.Should().Be(AccountStatus.Cancelled);
        
        _mockUow.Verify(x => x.BeginTransactionAsync(), Times.Once);
        _mockUow.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task TransactionOnCancelledAccount_ShouldThrowValidationException()
    {
        // Arrange
        var cancelledAccount = new SavingsAccount { Id = 1, AccountNumber = "123123123", Status = AccountStatus.Cancelled, Balance = 0 };
        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { cancelledAccount });

        var handler = new DepositCommandHandler(_mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object);
        var command = new DepositCommand { AccountNumber = "123123123", Amount = 100 };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("No se pueden realizar operaciones sobre una cuenta cancelada.");
    }
}
