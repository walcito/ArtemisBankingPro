using ArtemisBankingPro.Core.Application.Features.HermesPay.Commands.ProcessPayment;
using ArtemisBankingPro.Core.Application.Features.HermesPay.Queries.GetCommerceTransactions;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests.Card
{
    public class ProcessPaymentCommandHandlerTests
    {
        private readonly Mock<ICreditCardRepository> _cardRepoMock = new();
        private readonly Mock<ICardConsumptionRepository> _consumptionRepoMock = new();
        private readonly Mock<ICardProcessingLogRepository> _logRepoMock = new();
        private readonly Mock<IGenericRepository<Trade>> _tradeRepoMock = new();
        private readonly Mock<IGenericRepository<SavingsAccount>> _savingsRepoMock = new();
        private readonly Mock<IGenericRepository<Transaction>> _txRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<ICvcHashService> _cvcMock = new();
        private readonly Mock<IEmailService> _emailMock = new();
        private readonly Mock<ILogger<ProcessPaymentCommandHandler>> _loggerMock = new();

        [Fact]
        public async Task ProcessPayment_ValidDataAndAvailableCredit_ApprovesPaymentAndCreditsCommerce()
        {
            // Arrange
            var commerce = new Trade
            {
                Id = 5,
                Name = "Tienda Demo",
                IsActive = true,
                AssociatedUserId = 10,
                Mail = "comercio@demo.com"
            };

            var commerceMainAccount = new SavingsAccount
            {
                Id = 100,
                UserId = 10,
                Balance = 5000m,
                Type = AccountType.Principal,
                Status = AccountStatus.Active
            };

            var clientUser = new User
            {
                Id = 2,
                FirstName = "María",
                LastName = "Gómez",
                Email = "maria@test.com",
                IsActive = true
            };

            var expirationDate = DateTime.UtcNow.AddYears(2);
            var card = new CreditCard
            {
                Id = 1,
                UserId = 2,
                CardNumber = "4000123456789010",
                ExpirationDate = expirationDate,
                CvcHash = "STORED_HASH",
                CreditLimit = 50000m,
                CurrentDebt = 10000m,
                Status = CardStatus.Active
            };

            _tradeRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(commerce);
            _savingsRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { commerceMainAccount });
            _cardRepoMock.Setup(r => r.GetByCardNumberAsync("4000123456789010")).ReturnsAsync(card);
            _userRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(clientUser);
            _cvcMock.Setup(c => c.VerifyCvc("859", "STORED_HASH")).Returns(true);

            var handler = new ProcessPaymentCommandHandler(
                _cardRepoMock.Object, _consumptionRepoMock.Object, _logRepoMock.Object,
                _tradeRepoMock.Object, _savingsRepoMock.Object, _txRepoMock.Object,
                _userRepoMock.Object, _cvcMock.Object, _emailMock.Object, _loggerMock.Object);

            var command = new ProcessPaymentCommand
            {
                CommerceId = 5,
                CardNumber = "4000123456789010",
                MonthExpirationCard = expirationDate.ToString("MM"),
                YearExpirationCard = expirationDate.ToString("yyyy"),
                Cvc = "859",
                TransactionAmount = 2500m,
                Description = "Compra de prueba"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsApproved);
            Assert.Equal(12500m, card.CurrentDebt);
            Assert.Equal(7500m, commerceMainAccount.Balance);
            _cardRepoMock.Verify(r => r.UpdateAsync(card), Times.Once);
            _savingsRepoMock.Verify(r => r.UpdateAsync(commerceMainAccount), Times.Once);
            _consumptionRepoMock.Verify(r => r.AddAsync(It.Is<CardConsumption>(c => c.Status == ConsumptionStatus.Approved)), Times.Once);
            _txRepoMock.Verify(r => r.AddAsync(It.Is<Transaction>(t => t.Type == TransactionType.Credit)), Times.Once);
        }

        [Fact]
        public async Task ProcessPayment_InsufficientCredit_RejectsPaymentWithoutAffectingBalances()
        {
            // Arrange
            var commerce = new Trade { Id = 1, Name = "Demo", IsActive = true, AssociatedUserId = 10 };
            var savingsAcc = new SavingsAccount { Id = 5, UserId = 10, Type = AccountType.Principal, Status = AccountStatus.Active, Balance = 1000m };
            var expDate = DateTime.UtcNow.AddYears(1);

            var card = new CreditCard
            {
                Id = 1,
                UserId = 2,
                CardNumber = "4000123456789010",
                ExpirationDate = expDate,
                CvcHash = "STORED_HASH",
                CreditLimit = 5000m,
                CurrentDebt = 4900m,
                Status = CardStatus.Active
            };

            _tradeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(commerce);
            _savingsRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SavingsAccount> { savingsAcc });
            _cardRepoMock.Setup(r => r.GetByCardNumberAsync("4000123456789010")).ReturnsAsync(card);
            _cvcMock.Setup(c => c.VerifyCvc("123", "STORED_HASH")).Returns(true);

            var handler = new ProcessPaymentCommandHandler(
                _cardRepoMock.Object, _consumptionRepoMock.Object, _logRepoMock.Object,
                _tradeRepoMock.Object, _savingsRepoMock.Object, _txRepoMock.Object,
                _userRepoMock.Object, _cvcMock.Object, _emailMock.Object, _loggerMock.Object);

            var command = new ProcessPaymentCommand
            {
                CommerceId = 1,
                CardNumber = "4000123456789010",
                MonthExpirationCard = expDate.ToString("MM"),
                YearExpirationCard = expDate.ToString("yyyy"),
                Cvc = "123",
                TransactionAmount = 500m
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsApproved);
            Assert.Contains("excede el crédito disponible", result.Message);
            Assert.Equal(4900m, card.CurrentDebt);
            Assert.Equal(1000m, savingsAcc.Balance);
            _consumptionRepoMock.Verify(r => r.AddAsync(It.Is<CardConsumption>(c => c.Status == ConsumptionStatus.Rejected)), Times.Once);
        }

        [Fact]
        public async Task GetCommerceTransactions_ReturnsFilteredAndMaskedTransactions()
        {
            // Arrange
            var commerce = new Trade { Id = 3, Name = "Restaurante Central" };
            var card = new CreditCard
            {
                Id = 1,
                UserId = 2,
                CardNumber = "4000123456789010",
                ExpirationDate = DateTime.UtcNow.AddYears(1),
                CvcHash = "HASH",
                CreditLimit = 50000m,
                CurrentDebt = 0m,
                Status = CardStatus.Active
            };

            var consumptions = new List<CardConsumption>
            {
                new CardConsumption { Id = 1, CardId = 1, CommerceId = 3, Amount = 1500m, Status = ConsumptionStatus.Approved, TransactionDate = DateTime.UtcNow },
                new CardConsumption { Id = 2, CardId = 1, CommerceId = 3, Amount = 800m, Status = ConsumptionStatus.Rejected, TransactionDate = DateTime.UtcNow.AddHours(-1) }
            };

            _tradeRepoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(commerce);
            _consumptionRepoMock.Setup(r => r.GetByCommerceIdAsync(3)).ReturnsAsync(consumptions);
            _cardRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(card);

            var handler = new GetCommerceTransactionsQueryHandler(
                _consumptionRepoMock.Object, _cardRepoMock.Object, _tradeRepoMock.Object);

            var query = new GetCommerceTransactionsQuery { CommerceId = 3, Page = 1, PageSize = 20 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalRecords);
            Assert.Equal("9010", result.Data[0].CardLastFourDigits);
            Assert.Equal("APROBADO", result.Data[0].Status);
            Assert.Equal("RECHAZADO", result.Data[1].Status);
        }
    }
}