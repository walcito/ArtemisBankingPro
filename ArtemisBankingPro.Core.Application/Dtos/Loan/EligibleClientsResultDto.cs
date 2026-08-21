namespace ArtemisBankingPro.Core.Application.Dtos.Loan;

public class EligibleClientsResultDto
{
    public List<EligibleClientDto> Clients { get; set; } = new();
    public decimal AverageDebt { get; set; }
}
