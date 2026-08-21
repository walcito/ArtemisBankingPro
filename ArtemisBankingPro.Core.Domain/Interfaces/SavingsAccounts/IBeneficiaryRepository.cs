using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;

namespace ArtemisBankingPro.Core.Domain.Interfaces.SavingsAccounts
{
    public interface IBeneficiaryRepository : IGenericRepository<Beneficiary>
    {
        Task<IReadOnlyList<Beneficiary>> GetByOwnerUserIdAsync(int ownerUserId);
        Task<Beneficiary?> GetByOwnerAndAccountNumberAsync(int ownerUserId, string accountNumber);
        Task<bool> ExistsForOwnerAsync(int ownerUserId, string accountNumber);
    }
}
