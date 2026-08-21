using ArtemisBankingPro.Core.Domain.Entities.Loans;

namespace ArtemisBankingPro.Core.Domain.Interfaces.Loans
{
    public interface IAmortizationInstallmentRepository : IGenericRepository<AmortizationInstallment>
    {
        Task<IReadOnlyList<AmortizationInstallment>> GetByLoanIdAsync(int loanId);
        Task<IReadOnlyList<AmortizationInstallment>> GetPendingInstallmentsByLoanIdAsync(int loanId);
    }
}
