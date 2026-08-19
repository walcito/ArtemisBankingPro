using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.ThirdPartyTransaction;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Interfaces;
using Moq;
using Xunit;

namespace ArtemisBankingPro.Tests.UnitTests.Cashier;

public class ThirdPartyTransactionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRegisterRejectedTransaction_WhenInsufficientFunds()
    {
        var source = new SavingsAccount { Id = 1, AccountNumber = "111", Balance = 50m, Status = AccountStatus.Active };
        var destination = new SavingsAccount { Id = 2, AccountNumber = "222", Balance = 0m, Status = AccountStatus.Active };

        var savingsRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        savingsRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { source, destination });

        var transactionRepoMock = new Mock<IGenericRepository<Transaction>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new ThirdPartyTransactionCommandHandler(
            savingsRepoMock.Object, transactionRepoMock.Object, unitOfWorkMock.Object);

        var command = new ThirdPartyTransactionCommand
        {
            SourceAccountNumber = "111",
            DestinationAccountNumber = "222",
            Amount = 200m,
            CashierUserId = 5
        };

        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

        // Se registró el intento rechazado, pero el balance no cambió
        Assert.Equal(50m, source.Balance);
        transactionRepoMock.Verify(r => r.AddAsync(It.Is<Transaction>(t => t.Status == TransactionStatus.Rejected)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMoveBalanceWithCrossedTransactions_WhenFundsAreSufficient()
    {
        var source = new SavingsAccount { Id = 1, AccountNumber = "111", Balance = 500m, Status = AccountStatus.Active };
        var destination = new SavingsAccount { Id = 2, AccountNumber = "222", Balance = 100m, Status = AccountStatus.Active };

        var savingsRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        savingsRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { source, destination });

        var transactionRepoMock = new Mock<IGenericRepository<Transaction>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new ThirdPartyTransactionCommandHandler(
            savingsRepoMock.Object, transactionRepoMock.Object, unitOfWorkMock.Object);

        var command = new ThirdPartyTransactionCommand
        {
            SourceAccountNumber = "111",
            DestinationAccountNumber = "222",
            Amount = 150m,
            CashierUserId = 5
        };

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(350m, source.Balance);
        Assert.Equal(250m, destination.Balance);
        transactionRepoMock.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Exactly(2));
    }
}