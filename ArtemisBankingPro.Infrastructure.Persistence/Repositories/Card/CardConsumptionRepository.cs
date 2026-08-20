using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.Card
{
    public class CardConsumptionRepository : GenericRepository<CardConsumption>, ICardConsumptionRepository
    {
        public CardConsumptionRepository(ArtemisBankingDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<CardConsumption>> GetByCardIdAsync(int cardId)
        {
            return await _dbContext.Set<CardConsumption>()
                .Where(c => c.CardId == cardId)
                .OrderByDescending(c => c.TransactionDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<CardConsumption>> GetByCommerceIdAsync(int commerceId)
        {
            return await _dbContext.Set<CardConsumption>()
                .Where(c => c.CommerceId == commerceId)
                .OrderByDescending(c => c.TransactionDate)
                .ToListAsync();
        }

        public async Task<int> GetApprovedCountByCardIdAsync(int cardId)
        {
            return await _dbContext.Set<CardConsumption>()
                .Where(c => c.CardId == cardId && c.Status == ConsumptionStatus.Approved)
                .CountAsync();
        }

        public async Task<int> GetRejectedCountByCardIdAsync(int cardId)
        {
            return await _dbContext.Set<CardConsumption>()
                .Where(c => c.CardId == cardId && c.Status == ConsumptionStatus.Rejected)
                .CountAsync();
        }
    }
}
