using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Users;

namespace ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;

public class SavingsAccount
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0.00m;
    public AccountType Type { get; set; } = AccountType.Secondary;
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public int UserId { get; set; }
    public User? User { get; set; }
}
