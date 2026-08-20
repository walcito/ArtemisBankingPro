using ArtemisBankingPro.Core.Domain.Entities.Users;

namespace ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;

public class Beneficiary
{
    public int Id { get; set; }
    public int OwnerUserId { get; set; }
    public User? OwnerUser { get; set; }
    public string BeneficiaryAccountNumber { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
}
