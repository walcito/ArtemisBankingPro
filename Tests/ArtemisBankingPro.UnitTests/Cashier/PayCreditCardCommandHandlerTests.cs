using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayCreditCard;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using ArtemisBankingPro.Core.Domain.Interfaces;
using Moq;
using Xunit;

namespace ArtemisBankingPro.Tests.UnitTests.Cashier;

public class PayCreditCardCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrow_WhenAmountExceedsCurrentDebt()
    {
        var account = new SavingsAccount { Id = 1, AccountNumber = "111", Balance = 1000m, Status = AccountStatus.Active };
        var card = new CreditCard
        {
            Id = 1, UserId = 1, CardNumber = "1234567890123456",
            ExpirationDate = DateTime.UtcNow.AddYears(2), CvcHash = "hash",
            CreditLimit = 1000m, CurrentDebt = 100m, Status = CardStatus.Active
        };

        var savingsRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        savingsRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { account });

        var cardRepoMock = new Mock<IGenericRepository<CreditCard>>();
        cardRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(card);

        var transactionRepoMock = new Mock<IGenericRepository<Transaction>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new PayCreditCardCommandHandler(
            savingsRepoMock.Object, cardRepoMock.Object, transactionRepoMock.Object, unitOfWorkMock.Object);

        var command = new PayCreditCardCommand
        {
            SourceAccountNumber = "111",
            CreditCardId = 1,
            Amount = 150m, // más que la deuda actual (100)
            CashierUserId = 5
        };

        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));

        // La deuda no debió modificarse
        Assert.Equal(100m, card.CurrentDebt);
    }

    [Fact]
    public async Task Handle_ShouldReduceDebtAndIncreaseAvailableCredit_WhenPaymentIsValid()
    {
        var account = new SavingsAccount { Id = 1, AccountNumber = "111", Balance = 1000m, Status = AccountStatus.Active };
        var card = new CreditCard
        {
            Id = 1, UserId = 1, CardNumber = "1234567890123456",
            ExpirationDate = DateTime.UtcNow.AddYears(2), CvcHash = "hash",
            CreditLimit = 1000m, CurrentDebt = 300m, Status = CardStatus.Active
        };

        var savingsRepoMock = new Mock<IGenericRepository<SavingsAccount>>();
        savingsRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { account });

        var cardRepoMock = new Mock<IGenericRepository<CreditCard>>();
        cardRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(card);

        var transactionRepoMock = new Mock<IGenericRepository<Transaction>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var handler = new PayCreditCardCommandHandler(
            savingsRepoMock.Object, cardRepoMock.Object, transactionRepoMock.Object, unitOfWorkMock.Object);

        var command = new PayCreditCardCommand
        {
            SourceAccountNumber = "111",
            CreditCardId = 1,
            Amount = 100m,
            CashierUserId = 5
        };

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(200m, card.CurrentDebt);
        Assert.Equal(800m, card.AvailableCredit); // 1000 límite - 200 deuda restante
        Assert.Equal(900m, account.Balance);
    }
}