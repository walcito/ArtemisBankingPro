using ArtemisBankingPro.Core.Domain.Common.Enums;

namespace ArtemisBankingPro.Core.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IdentificationNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public Roles Role { get; set; }
    public bool IsActive { get; set; } = false;
    public int? TradeId { get; set; }
    public Trade? Trade { get; set; }
    public ICollection<SavingsAccount> SavingsAccounts { get; set; } = new List<SavingsAccount>();
}