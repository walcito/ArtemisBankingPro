namespace ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetBeneficiaries;

public class BeneficiaryDto
{
    public int Id { get; set; }
    public string Alias { get; set; } = string.Empty;
    public string BeneficiaryAccountNumber { get; set; } = string.Empty;
}