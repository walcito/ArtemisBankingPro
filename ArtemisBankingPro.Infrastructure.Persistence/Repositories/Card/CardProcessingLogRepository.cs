using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.Card
{
    public class CardProcessingLogRepository : GenericRepository<CardProcessingLog>, ICardProcessingLogRepository
    {
        public CardProcessingLogRepository(ArtemisBankingDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<CardProcessingLog>> GetByCardIdAsync(int cardId)
        {
            return await _dbContext.Set<CardProcessingLog>()
                .Where(c => c.CardId == cardId)
                .OrderByDescending(c => c.ProcessedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<CardProcessingLog>> GetByCommerceIdAsync(int commerceId)
        {
            return await _dbContext.Set<CardProcessingLog>()
                .Where(c => c.CommerceId == commerceId)
                .OrderByDescending(c => c.ProcessedAt)
                .ToListAsync();
        }

        public async Task<CardProcessingLog?> GetLastProcessingByCardAsync(int cardId)
        {
            return await _dbContext.Set<CardProcessingLog>()
                .Where(c => c.CardId == cardId)
                .OrderByDescending(c => c.ProcessedAt)
                .FirstOrDefaultAsync();
        }
    }
}
