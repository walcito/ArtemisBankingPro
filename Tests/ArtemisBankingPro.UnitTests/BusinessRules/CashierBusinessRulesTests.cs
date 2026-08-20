using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Deposit;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Withdraw;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests.BusinessRules;

public class CashierBusinessRulesTests
{
    private readonly Mock<IGenericRepository<SavingsAccount>> _mockAccountRepo;
    private readonly Mock<IGenericRepository<Transaction>> _mockTransactionRepo;
    private readonly Mock<IUnitOfWork> _mockUow;

    public CashierBusinessRulesTests()
    {
        _mockAccountRepo = new Mock<IGenericRepository<SavingsAccount>>();
        _mockTransactionRepo = new Mock<IGenericRepository<Transaction>>();
        _mockUow = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Deposit_ShouldIncrementBalance_Correctly()
    {
        // Arrange
        var account = new SavingsAccount { Id = 1, AccountNumber = "123456789", Balance = 500, Status = AccountStatus.Active };
        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { account });

        var handler = new DepositCommandHandler(_mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object);
        var command = new DepositCommand { AccountNumber = "123456789", Amount = 1500 };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        account.Balance.Should().Be(2000);
        _mockAccountRepo.Verify(x => x.Update(It.Is<SavingsAccount>(a => a.Balance == 2000)), Times.Once);
        _mockUow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Withdraw_WithSufficientFunds_ShouldDecrementBalance_Correctly()
    {
        // Arrange
        var account = new SavingsAccount { Id = 1, AccountNumber = "987654321", Balance = 1500, Status = AccountStatus.Active };
        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { account });

        var handler = new WithdrawCommandHandler(_mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object);
        var command = new WithdrawCommand { AccountNumber = "987654321", Amount = 500 };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        account.Balance.Should().Be(1000);
        _mockAccountRepo.Verify(x => x.Update(It.Is<SavingsAccount>(a => a.Balance == 1000)), Times.Once);
        _mockUow.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Withdraw_WithInsufficientFunds_ShouldThrowException()
    {
        // Arrange
        var account = new SavingsAccount { Id = 1, AccountNumber = "987654321", Balance = 100, Status = AccountStatus.Active };
        _mockAccountRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { account });

        var handler = new WithdrawCommandHandler(_mockAccountRepo.Object, _mockTransactionRepo.Object, _mockUow.Object);
        var command = new WithdrawCommand { AccountNumber = "987654321", Amount = 500 };

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("*Fondos insuficientes*");
    }
}
