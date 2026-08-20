using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Deposit;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using Moq;
using Xunit;

namespace ArtemisBankingPro.Tests.UnitTests.SavingsAccounts;

public class DepositCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenAccountIsCancelled()
    {
        // Arrange
        var cancelledAccount = new SavingsAccount
        {
            Id = 1,
            AccountNumber = "987654321",
            Balance = 0m,
            Status = AccountStatus.Cancelled
        };

        var savingsRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        savingsRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<SavingsAccount> { cancelledAccount });

        var transactionRepoMock = new Mock<IGenericRepository<Transaction>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new DepositCommandHandler(
            savingsRepoMock.Object, transactionRepoMock.Object, unitOfWorkMock.Object);

        var command = new DepositCommand { AccountNumber = "987654321", Amount = 100m };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

        transactionRepoMock.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Never);
    }
}