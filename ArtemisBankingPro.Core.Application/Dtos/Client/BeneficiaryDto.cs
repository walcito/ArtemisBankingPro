namespace ArtemisBankingPro.Core.Application.Dtos.Client;

public class BeneficiaryDto
{
    public int Id { get; set; }
    public string Alias { get; set; } = string.Empty;
    public string BeneficiaryAccountNumber { get; set; } = string.Empty;
}
