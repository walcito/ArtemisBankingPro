using ArtemisBankingPro.Core.Domain.Entities.Card;

namespace ArtemisBankingPro.Core.Domain.Interfaces.Card
{
    public interface ICardConsumptionRepository : IGenericRepository<CardConsumption>
    {
        Task<IReadOnlyList<CardConsumption>> GetByCardIdAsync(int cardId);
        Task<IReadOnlyList<CardConsumption>> GetByCommerceIdAsync(int commerceId);
        Task<int> GetApprovedCountByCardIdAsync(int cardId);
        Task<int> GetRejectedCountByCardIdAsync(int cardId);
    }
}
