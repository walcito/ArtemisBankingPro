using ArtemisBankingPro.Core.Domain.Entities.Loans;

namespace ArtemisBankingPro.Core.Domain.Interfaces.Loans
{
    public interface ILoanRepository : IGenericRepository<Loan>
    {
        Task<Loan?> GetByLoanNumberAsync(string loanNumber);
        Task<IReadOnlyList<Loan>> GetByUserIdAsync(int userId);
        Task<bool> LoanNumberExistsAsync(string loanNumber);
        Task<IReadOnlyList<Loan>> GetAllWithUserAndInstallmentsAsync();
        Task<Loan?> GetByIdWithInstallmentsAsync(int id);
        Task<bool> HasActiveLoanAsync(int userId);
    }
}
