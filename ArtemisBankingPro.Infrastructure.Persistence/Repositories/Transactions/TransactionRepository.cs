using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using ArtemisBankingPro.Core.Domain.Interfaces.Transactions;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.Transactions
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ArtemisBankingDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<Transaction>> GetBySavingsAccountIdAsync(int savingsAccountId)
        {
            return await _dbContext.Set<Transaction>()
                .Where(t => t.SavingsAccountId == savingsAccountId)
                .Include(t => t.SavingsAccount)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Transaction>> GetByCashierIdAndDateAsync(int cashierId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _dbContext.Set<Transaction>()
                .Where(t => t.PerformedByCashierId == cashierId && t.Date >= startOfDay && t.Date < endOfDay)
                .Include(t => t.SavingsAccount)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Transaction>> GetAllWithAccountAsync()
        {
            return await _dbContext.Set<Transaction>()
                .Include(t => t.SavingsAccount)
                .Include(t => t.PerformedByCashier)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }
    }
}
