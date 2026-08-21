namespace ArtemisBankingPro.Core.Application.ViewModels.Loan;

public class HighRiskWarningViewModel
{
    public int UserId { get; set; }
    public decimal DisbursedAmount { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int TermInMonths { get; set; }
    public string WarningMessage { get; set; } = string.Empty;
    public string ClientFullName { get; set; } = string.Empty;
}
