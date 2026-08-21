using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Interfaces.SavingsAccounts;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.SavingsAccount
{
    public class BeneficiaryRepository : GenericRepository<Beneficiary>, IBeneficiaryRepository
    {
        public BeneficiaryRepository(ArtemisBankingDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<Beneficiary>> GetByOwnerUserIdAsync(int ownerUserId)
        {
            return await _dbContext.Set<Beneficiary>()
                .Where(b => b.OwnerUserId == ownerUserId)
                .Include(b => b.OwnerUser)
                .ToListAsync();
        }

        public async Task<Beneficiary?> GetByOwnerAndAccountNumberAsync(int ownerUserId, string accountNumber)
        {
            return await _dbContext.Set<Beneficiary>()
                .Include(b => b.OwnerUser)
                .FirstOrDefaultAsync(b => b.OwnerUserId == ownerUserId && b.BeneficiaryAccountNumber == accountNumber);
        }

        public async Task<bool> ExistsForOwnerAsync(int ownerUserId, string accountNumber)
        {
            return await _dbContext.Set<Beneficiary>()
                .AnyAsync(b => b.OwnerUserId == ownerUserId && b.BeneficiaryAccountNumber == accountNumber);
        }
    }
}
