namespace ArtemisBankingPro.Core.Application.ViewModels.Loan;

public class EligibleClientViewModel
{
    public int Id { get; set; }
    public string IdentificationNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalDebt { get; set; }
}
