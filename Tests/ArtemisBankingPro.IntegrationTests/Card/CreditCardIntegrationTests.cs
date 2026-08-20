using ArtemisBankingPro.Core.Domain.Common.Enums.Users;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using ArtemisBankingPro.Infrastructure.Persistence.Repositories.Card;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ArtemisBankingPro.IntegrationTests.Card
{
    public class CreditCardIntegrationTests
    {
        private ArtemisBankingDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ArtemisBankingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ArtemisBankingDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task AddCreditCard_ShouldPersistAndRetrieveWithUser()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var cardRepository = new CreditCardRepository(context);

            var user = new User
            {
                Id = 10,
                FirstName = "Laura",
                LastName = "Méndez",
                IdentificationNumber = "40200000010",
                Email = "laura@example.com",
                Username = "lauram",
                Role = Roles.Client,
                IsActive = true
            };
            context.Set<User>().Add(user);
            await context.SaveChangesAsync();

            var card = new CreditCard
            {
                UserId = user.Id,
                CardNumber = "4000123456789010",
                ExpirationDate = DateTime.UtcNow.AddYears(3),
                CvcHash = "ABC123HASH",
                CreditLimit = 75000m,
                CurrentDebt = 0m,
                Status = CardStatus.Active,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            // Act - Guardar
            await cardRepository.AddAsync(card);
            context.ChangeTracker.Clear();

            // Act - Recuperar
            var retrievedCard = await cardRepository.GetByCardNumberAsync("4000123456789010");

            // Assert
            Assert.NotNull(retrievedCard);
            Assert.Equal(user.Id, retrievedCard.UserId);
            Assert.NotNull(retrievedCard.User);
            Assert.Equal("Laura", retrievedCard.User.FirstName);
            Assert.Equal(75000m, retrievedCard.CreditLimit);
            Assert.Equal(75000m, retrievedCard.AvailableCredit);
            Assert.Equal(CardStatus.Active, retrievedCard.Status);
        }

        [Fact]
        public async Task AddCardConsumption_ShouldPersistAndRetrieveByCardId()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var consumptionRepository = new CardConsumptionRepository(context);

            var card = new CreditCard
            {
                Id = 20,
                UserId = 1,
                CardNumber = "4000123456789020",
                ExpirationDate = DateTime.UtcNow.AddYears(3),
                CvcHash = "HASH",
                CreditLimit = 50000m,
                CurrentDebt = 3500m,
                Status = CardStatus.Active,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            context.Set<CreditCard>().Add(card);

            var consumption = new CardConsumption
            {
                CardId = card.Id,
                CommerceId = 2,
                Amount = 3500m,
                Status = ConsumptionStatus.Approved,
                TransactionDate = DateTime.UtcNow,
                Description = "Supermercado Nacional",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            await consumptionRepository.AddAsync(consumption);
            context.ChangeTracker.Clear();

            var consumptions = await consumptionRepository.GetByCardIdAsync(card.Id);

            // Assert
            Assert.Single(consumptions);
            Assert.Equal(3500m, consumptions[0].Amount);
            Assert.Equal(ConsumptionStatus.Approved, consumptions[0].Status);
            Assert.Equal("Supermercado Nacional", consumptions[0].Description);
        }

        [Fact]
        public async Task UpdateCardLimitAndCancel_ShouldPersistCorrectly()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var cardRepository = new CreditCardRepository(context);

            var card = new CreditCard
            {
                Id = 30,
                UserId = 1,
                CardNumber = "4000123456789030",
                ExpirationDate = DateTime.UtcNow.AddYears(3),
                CvcHash = "HASH",
                CreditLimit = 20000m,
                CurrentDebt = 0m,
                Status = CardStatus.Active,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            await cardRepository.AddAsync(card);
            context.ChangeTracker.Clear();

            // Act - Modificar Límite
            var dbCard = await cardRepository.GetByIdAsync(30);
            Assert.NotNull(dbCard);
            dbCard.CreditLimit = 40000m;
            dbCard.Status = CardStatus.Cancelled;
            dbCard.CancelledAt = DateTime.UtcNow;

            await cardRepository.UpdateAsync(dbCard);
            context.ChangeTracker.Clear();

            // Assert
            var updatedCard = await cardRepository.GetByIdAsync(30);
            Assert.NotNull(updatedCard);
            Assert.Equal(40000m, updatedCard.CreditLimit);
            Assert.Equal(CardStatus.Cancelled, updatedCard.Status);
            Assert.NotNull(updatedCard.CancelledAt);
        }
    }
}
