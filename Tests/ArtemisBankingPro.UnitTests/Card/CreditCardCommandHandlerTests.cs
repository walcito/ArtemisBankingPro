using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Application.Features.Card.Commands.AssignCreditCard;
using ArtemisBankingPro.Core.Application.Features.Card.Commands.CancelCreditCard;
using ArtemisBankingPro.Core.Application.Features.Card.Commands.UpdateCredit;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ArtemisBankingPro.UnitTests.Card
{
    public class CreditCardCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICreditCardRepository> _cardRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ICardNumberGeneratorService> _cardGenMock;
        private readonly Mock<ICvcHashService> _cvcHashMock;
        private readonly Mock<IEmailService> _emailMock;
        private readonly Mock<ILogger<AssignCreditCardCommandHandler>> _assignLoggerMock;
        private readonly Mock<ILogger<UpdateCreditLimitCommandHandler>> _updateLoggerMock;

        public CreditCardCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _cardRepoMock = new Mock<ICreditCardRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _cardGenMock = new Mock<ICardNumberGeneratorService>();
            _cvcHashMock = new Mock<ICvcHashService>();
            _emailMock = new Mock<IEmailService>();
            _assignLoggerMock = new Mock<ILogger<AssignCreditCardCommandHandler>>();
            _updateLoggerMock = new Mock<ILogger<UpdateCreditLimitCommandHandler>>();
        }

        [Fact]
        public async Task AssignCreditCard_ValidActiveClient_ReturnsCreatedCardResponse()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "Juan",
                LastName = "Pérez",
                IdentificationNumber = "40200000001",
                Email = "juan@test.com",
                Username = "juanp",
                Role = Roles.Client,
                IsActive = true
            };

            var savedCard = new CreditCard
            {
                Id = 10,
                UserId = 1,
                User = user,
                CardNumber = "4000123456789010",
                ExpirationDate = DateTime.UtcNow.AddYears(3),
                CvcHash = "HASHED_CVC",
                CreditLimit = 50000m,
                CurrentDebt = 0m,
                Status = CardStatus.Active,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            var expectedResponse = new CreateCreditCardResponseDto
            {
                Id = 10,
                ClientId = 1,
                ClientFullName = "Juan Pérez",
                CreditLimit = 50000m,
                AvailableCredit = 50000m,
                CurrentDebt = 0m,
                LastFourDigits = "9010",
                MaskedCardNumber = "************9010",
                Status = CardStatus.Active
            };

            _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _cardGenMock.Setup(g => g.GenerateUniqueCardNumberAsync()).ReturnsAsync("4000123456789010");
            _cvcHashMock.Setup(h => h.GenerateHashedCvc()).Returns(("123", "HASHED_CVC"));
            _cardRepoMock.Setup(r => r.GetByCardNumberAsync("4000123456789010")).ReturnsAsync(savedCard);
            _mapperMock.Setup(m => m.Map<CreateCreditCardResponseDto>(It.IsAny<CreditCard>())).Returns(expectedResponse);

            var handler = new AssignCreditCardCommandHandler(
                _cardRepoMock.Object, _userRepoMock.Object, _cardGenMock.Object, _cvcHashMock.Object,
                _emailMock.Object, _mapperMock.Object, _assignLoggerMock.Object);

            var command = new AssignCreditCardCommand { ClientId = 1, CreditLimit = 50000m };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(50000m, result.CreditLimit);
            Assert.Equal(50000m, result.AvailableCredit);
            Assert.Equal("9010", result.LastFourDigits);
            _cardRepoMock.Verify(r => r.AddAsync(It.IsAny<CreditCard>()), Times.Once);
        }

        [Fact]
        public async Task AssignCreditCard_InactiveClient_ThrowsException()
        {
            // Arrange
            var inactiveUser = new User
            {
                Id = 2,
                FirstName = "Maria",
                LastName = "Gomez",
                IsActive = false
            };

            _userRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(inactiveUser);

            var handler = new AssignCreditCardCommandHandler(
                _cardRepoMock.Object, _userRepoMock.Object, _cardGenMock.Object, _cvcHashMock.Object,
                _emailMock.Object, _mapperMock.Object, _assignLoggerMock.Object);

            var command = new AssignCreditCardCommand { ClientId = 2, CreditLimit = 30000m };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Contains("clientes activos", ex.Message);
        }

        [Fact]
        public async Task UpdateCreditLimit_ValidNewLimit_UpdatesLimitSuccessfully()
        {
            // Arrange
            var card = new CreditCard
            {
                Id = 5,
                UserId = 1,
                CardNumber = "4000123456789010",
                ExpirationDate = DateTime.UtcNow.AddYears(2),
                CvcHash = "HASHED_CVC",
                CreditLimit = 50000m,
                CurrentDebt = 10000m,
                Status = CardStatus.Active
            };

            var expectedDto = new ListCreditCardDto { Id = 5, CreditLimit = 75000m };

            _cardRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(card);
            _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1, Email = "juan@test.com", FirstName = "Juan", LastName = "Pérez" });
            _mapperMock.Setup(m => m.Map<ListCreditCardDto>(It.IsAny<CreditCard>())).Returns(expectedDto);

            var handler = new UpdateCreditLimitCommandHandler(
                _cardRepoMock.Object, _userRepoMock.Object, _emailMock.Object, _mapperMock.Object, _updateLoggerMock.Object);

            var command = new UpdateCreditLimitCommand { CardId = 5, CreditLimit = 75000m };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(75000m, card.CreditLimit);
            _cardRepoMock.Verify(r => r.UpdateAsync(card), Times.Once);
        }

        [Fact]
        public async Task UpdateCreditLimit_LimitBelowCurrentDebt_ThrowsException()
        {
            // Arrange
            var card = new CreditCard
            {
                Id = 5,
                UserId = 1,
                CardNumber = "4000123456789010",
                ExpirationDate = DateTime.UtcNow.AddYears(2),
                CvcHash = "HASHED_CVC",
                CreditLimit = 50000m,
                CurrentDebt = 20000m,
                Status = CardStatus.Active
            };

            _cardRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(card);

            var handler = new UpdateCreditLimitCommandHandler(
                _cardRepoMock.Object, _userRepoMock.Object, _emailMock.Object, _mapperMock.Object, _updateLoggerMock.Object);

            var command = new UpdateCreditLimitCommand { CardId = 5, CreditLimit = 15000m };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Contains("inferior al monto adeudado", ex.Message);
        }

        [Fact]
        public async Task CancelCreditCard_NoDebt_CancelsCardSuccessfully()
        {
            // Arrange
            var card = new CreditCard
            {
                Id = 8,
                UserId = 1,
                CardNumber = "4000123456789010",
                ExpirationDate = DateTime.UtcNow.AddYears(2),
                CvcHash = "HASHED_CVC",
                CreditLimit = 50000m,
                CurrentDebt = 0m,
                Status = CardStatus.Active
            };

            var expectedDto = new ListCreditCardDto { Id = 8, Status = CardStatus.Cancelled };

            _cardRepoMock.Setup(r => r.GetByIdAsync(8)).ReturnsAsync(card);
            _mapperMock.Setup(m => m.Map<ListCreditCardDto>(It.IsAny<CreditCard>())).Returns(expectedDto);

            var handler = new CancelCreditCardCommandHandler(_cardRepoMock.Object, _mapperMock.Object);
            var command = new CancelCreditCardCommand { CardId = 8 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CardStatus.Cancelled, card.Status);
            Assert.NotNull(card.CancelledAt);
            _cardRepoMock.Verify(r => r.UpdateAsync(card), Times.Once);
        }

        [Fact]
        public async Task CancelCreditCard_WithPendingDebt_ThrowsException()
        {
            // Arrange
            var card = new CreditCard
            {
                Id = 8,
                UserId = 1,
                CardNumber = "4000123456789010",
                ExpirationDate = DateTime.UtcNow.AddYears(2),
                CvcHash = "HASHED_CVC",
                CreditLimit = 50000m,
                CurrentDebt = 2500m,
                Status = CardStatus.Active
            };

            _cardRepoMock.Setup(r => r.GetByIdAsync(8)).ReturnsAsync(card);

            var handler = new CancelCreditCardCommandHandler(_cardRepoMock.Object, _mapperMock.Object);
            var command = new CancelCreditCardCommand { CardId = 8 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
            Assert.Contains("saldar la totalidad de la deuda", ex.Message);
        }
    }
}