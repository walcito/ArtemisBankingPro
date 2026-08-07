using ArtemisBankingPro.Core.Domain.Entities.Card;

namespace ArtemisBankingPro.Core.Domain.Interfaces.Card
{
    public interface ICreditCardRepository : IGenericRepository<CreditCard>
    {
        Task<CreditCard?> GetByCardNumberAsync(string cardNumber);
        Task<IReadOnlyList<CreditCard>> GetByUserIdAsync(int userId);
        Task<bool> CardNumberExistsAsync(string cardNumber);
    }
}
