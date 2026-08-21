using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;

namespace ArtemisBankingPro.Core.Application.ViewModels.Loan;

public class LoanInstallmentViewModel
{
    public int Id { get; set; }
    public int InstallmentNumber { get; set; }
    public decimal InstallmentAmount { get; set; }
    public decimal Interest { get; set; }
    public decimal PrincipalAmortization { get; set; }
    public decimal PendingBalance { get; set; }
    public InstallmentStatus Status { get; set; }
    public decimal PaidAmount { get; set; }
}
