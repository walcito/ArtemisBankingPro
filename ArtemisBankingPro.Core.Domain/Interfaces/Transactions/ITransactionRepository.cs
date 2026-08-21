using ArtemisBankingPro.Core.Domain.Entities.Transactions;

namespace ArtemisBankingPro.Core.Domain.Interfaces.Transactions
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<IReadOnlyList<Transaction>> GetBySavingsAccountIdAsync(int savingsAccountId);
        Task<IReadOnlyList<Transaction>> GetByCashierIdAndDateAsync(int cashierId, DateTime date);
        Task<IReadOnlyList<Transaction>> GetAllWithAccountAsync();
    }
}
