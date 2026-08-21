using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;

namespace ArtemisBankingPro.Core.Application.ViewModels.Loan;

public class LoanViewModel
{
    public int Id { get; set; }
    public string LoanNumber { get; set; } = string.Empty;
    public decimal DisbursedAmount { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int TermInMonths { get; set; }
    public decimal CurrentDebt { get; set; }
    public LoanStatus Status { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserIdentificationNumber { get; set; } = string.Empty;
}
