using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;

namespace ArtemisBankingPro.Core.Application.Interfaces;

public interface ILoanService
{
    Task<PagedResult<LoanDto>> GetLoansAsync(int pageNumber, int pageSize, string? identificationNumber, LoanStatus? status);
    Task<DetallePrestamoDto?> GetLoanDetailAsync(int loanId);
    Task<EligibleClientsResultDto> GetEligibleClientsAsync(string? identificationNumber = null);
    Task<int> AssignLoanAsync(int userId, decimal disbursedAmount, decimal annualInterestRate, int termInMonths, bool confirmHighRisk);
    Task<bool> EditLoanRateAsync(int loanId, decimal newAnnualInterestRate);
}
