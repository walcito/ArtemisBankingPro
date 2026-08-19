using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using Moq;
using Xunit;

namespace ArtemisBankingPro.Tests.UnitTests.SavingsAccounts;

public class CancelSavingsAccountCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenAccountIsPrincipal()
    {
        var principalAccount = new SavingsAccount
        {
            Id = 1,
            AccountNumber = "123456789",
            Type = AccountType.Principal,
            Status = AccountStatus.Active,
            UserId = 1
        };

        var savingsRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        savingsRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<SavingsAccount> { principalAccount });

        var transactionRepoMock = new Mock<IGenericRepository<Transaction>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new CancelSavingsAccountCommandHandler(
            savingsRepoMock.Object, transactionRepoMock.Object, unitOfWorkMock.Object);

        var command = new CancelSavingsAccountCommand { AccountNumber = "123456789" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldTransferBalanceToPrincipal_WhenCancellingSecondaryWithBalance()
    {
        // Arrange
        var principalAccount = new SavingsAccount
        {
            Id = 1, AccountNumber = "111111111", Type = AccountType.Principal,
            Status = AccountStatus.Active, UserId = 1, Balance = 100m
        };

        var secondaryAccount = new SavingsAccount
        {
            Id = 2, AccountNumber = "222222222", Type = AccountType.Secondary,
            Status = AccountStatus.Active, UserId = 1, Balance = 500m
        };

        var savingsRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        savingsRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<SavingsAccount> { principalAccount, secondaryAccount });

        var transactionRepoMock = new Mock<IGenericRepository<Transaction>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new CancelSavingsAccountCommandHandler(
            savingsRepoMock.Object, transactionRepoMock.Object, unitOfWorkMock.Object);

        var command = new CancelSavingsAccountCommand { AccountNumber = "222222222" };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(0m, secondaryAccount.Balance);
        Assert.Equal(600m, principalAccount.Balance);
        Assert.Equal(AccountStatus.Cancelled, secondaryAccount.Status);
        transactionRepoMock.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Exactly(2));
    }
}