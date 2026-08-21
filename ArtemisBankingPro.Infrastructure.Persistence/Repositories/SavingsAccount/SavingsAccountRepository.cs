using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Interfaces.SavingsAccounts;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.SavingsAccount
{
    public class SavingsAccountRepository : GenericRepository<Core.Domain.Entities.SavingsAccounts.SavingsAccount>, ISavingsAccountRepository
    {
        public SavingsAccountRepository(ArtemisBankingDbContext dbContext) : base(dbContext) { }

        public async Task<Core.Domain.Entities.SavingsAccounts.SavingsAccount?> GetByAccountNumberAsync(string accountNumber)
        {
            return await _dbContext.Set<Core.Domain.Entities.SavingsAccounts.SavingsAccount>()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.AccountNumber == accountNumber);
        }

        public async Task<IReadOnlyList<Core.Domain.Entities.SavingsAccounts.SavingsAccount>> GetByUserIdAsync(int userId)
        {
            return await _dbContext.Set<Core.Domain.Entities.SavingsAccounts.SavingsAccount>()
                .Where(s => s.UserId == userId)
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<Core.Domain.Entities.SavingsAccounts.SavingsAccount?> GetPrimaryAccountByUserIdAsync(int userId)
        {
            return await _dbContext.Set<Core.Domain.Entities.SavingsAccounts.SavingsAccount>()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Type == Core.Domain.Common.Enums.SavingsAccounts.AccountType.Principal);
        }

        public async Task<bool> AccountNumberExistsAsync(string accountNumber)
        {
            return await _dbContext.Set<Core.Domain.Entities.SavingsAccounts.SavingsAccount>()
                .AnyAsync(s => s.AccountNumber == accountNumber);
        }

        public async Task<IReadOnlyList<Core.Domain.Entities.SavingsAccounts.SavingsAccount>> GetAllWithUserAsync()
        {
            return await _dbContext.Set<Core.Domain.Entities.SavingsAccounts.SavingsAccount>()
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<Core.Domain.Entities.SavingsAccounts.SavingsAccount?> GetByIdWithUserAsync(int id)
        {
            return await _dbContext.Set<Core.Domain.Entities.SavingsAccounts.SavingsAccount>()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
