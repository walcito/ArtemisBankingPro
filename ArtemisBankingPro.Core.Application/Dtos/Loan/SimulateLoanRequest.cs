namespace ArtemisBankingPro.Core.Application.Dtos.Loan;

public class SimulateLoanRequest
{
    public decimal DisbursedAmount { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int TermInMonths { get; set; }
}
