using ArtemisBankingPro.Core.Domain.Entities.Card;

namespace ArtemisBankingPro.Core.Domain.Interfaces.Card
{
    public interface ICardProcessingLogRepository : IGenericRepository<CardProcessingLog>
    {
        Task<IReadOnlyList<CardProcessingLog>> GetByCardIdAsync(int cardId);
        Task<IReadOnlyList<CardProcessingLog>> GetByCommerceIdAsync(int commerceId);
        Task<CardProcessingLog?> GetLastProcessingByCardAsync(int cardId);
    }
}
