using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Withdraw;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using Moq;
using Xunit;

namespace ArtemisBankingPro.Tests.UnitTests.SavingsAccounts;

public class WithdrawCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenBalanceIsInsufficient()
    {
        // Arrange
        var account = new SavingsAccount
        {
            Id = 1,
            AccountNumber = "123456789",
            Balance = 50m,
            Status = AccountStatus.Active
        };

        var savingsRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        savingsRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<SavingsAccount> { account });

        var transactionRepoMock = new Mock<IGenericRepository<Transaction>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new WithdrawCommandHandler(
            savingsRepoMock.Object, transactionRepoMock.Object, unitOfWorkMock.Object);

        var command = new WithdrawCommand { AccountNumber = "123456789", Amount = 200m };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

        // Verificamos que el balance NO se haya modificado, ya que la operación debió rechazarse
        Assert.Equal(50m, account.Balance);
    }
}