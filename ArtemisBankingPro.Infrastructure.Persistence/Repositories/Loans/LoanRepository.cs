using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Interfaces.Loans;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.Loans
{
    public class LoanRepository : GenericRepository<Loan>, ILoanRepository
    {
        public LoanRepository(ArtemisBankingDbContext dbContext) : base(dbContext) { }

        public async Task<Loan?> GetByLoanNumberAsync(string loanNumber)
        {
            return await _dbContext.Set<Loan>()
                .Include(l => l.User)
                .Include(l => l.Installments)
                .FirstOrDefaultAsync(l => l.LoanNumber == loanNumber);
        }

        public async Task<IReadOnlyList<Loan>> GetByUserIdAsync(int userId)
        {
            return await _dbContext.Set<Loan>()
                .Where(l => l.UserId == userId)
                .Include(l => l.User)
                .Include(l => l.Installments)
                .ToListAsync();
        }

        public async Task<bool> LoanNumberExistsAsync(string loanNumber)
        {
            return await _dbContext.Set<Loan>()
                .AnyAsync(l => l.LoanNumber == loanNumber);
        }

        public async Task<IReadOnlyList<Loan>> GetAllWithUserAndInstallmentsAsync()
        {
            return await _dbContext.Set<Loan>()
                .Include(l => l.User)
                .Include(l => l.Installments)
                .ToListAsync();
        }

        public async Task<Loan?> GetByIdWithInstallmentsAsync(int id)
        {
            return await _dbContext.Set<Loan>()
                .Include(l => l.User)
                .Include(l => l.Installments)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<bool> HasActiveLoanAsync(int userId)
        {
            return await _dbContext.Set<Loan>()
                .AnyAsync(l => l.UserId == userId && l.Status == LoanStatus.Active);
        }
    }
}
