namespace ArtemisBankingPro.Core.Application.ViewModels.Loan;

public class SelectClientForLoanViewModel
{
    public List<EligibleClientViewModel> Clients { get; set; } = new();
    public decimal AverageDebt { get; set; }
    public string? SearchIdentification { get; set; }
    public int? SelectedClientId { get; set; }
}
