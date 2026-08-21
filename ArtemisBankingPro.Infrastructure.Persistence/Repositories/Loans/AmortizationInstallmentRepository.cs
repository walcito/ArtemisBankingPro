using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Interfaces.Loans;
using ArtemisBankingPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories.Loans
{
    public class AmortizationInstallmentRepository : GenericRepository<AmortizationInstallment>, IAmortizationInstallmentRepository
    {
        public AmortizationInstallmentRepository(ArtemisBankingDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<AmortizationInstallment>> GetByLoanIdAsync(int loanId)
        {
            return await _dbContext.Set<AmortizationInstallment>()
                .Where(a => a.LoanId == loanId)
                .OrderBy(a => a.InstallmentNumber)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<AmortizationInstallment>> GetPendingInstallmentsByLoanIdAsync(int loanId)
        {
            return await _dbContext.Set<AmortizationInstallment>()
                .Where(a => a.LoanId == loanId && a.Status != InstallmentStatus.Paid)
                .OrderBy(a => a.InstallmentNumber)
                .ToListAsync();
        }
    }
}
