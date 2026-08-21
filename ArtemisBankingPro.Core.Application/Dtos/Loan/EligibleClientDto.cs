namespace ArtemisBankingPro.Core.Application.Dtos.Loan;

public class EligibleClientDto
{
    public int Id { get; set; }
    public string IdentificationNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalDebt { get; set; }
}
