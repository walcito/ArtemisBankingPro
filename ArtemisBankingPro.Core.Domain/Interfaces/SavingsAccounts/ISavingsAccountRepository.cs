using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;

namespace ArtemisBankingPro.Core.Domain.Interfaces.SavingsAccounts
{
    public interface ISavingsAccountRepository : IGenericRepository<SavingsAccount>
    {
        Task<SavingsAccount?> GetByAccountNumberAsync(string accountNumber);
        Task<IReadOnlyList<SavingsAccount>> GetByUserIdAsync(int userId);
        Task<SavingsAccount?> GetPrimaryAccountByUserIdAsync(int userId);
        Task<bool> AccountNumberExistsAsync(string accountNumber);
        Task<IReadOnlyList<SavingsAccount>> GetAllWithUserAsync();
        Task<SavingsAccount?> GetByIdWithUserAsync(int id);
    }
}
