using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetSavingsAccounts;

public class SavingsAccountDto
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public AccountType Type { get; set; }
    public AccountStatus Status { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserIdentificationNumber { get; set; } = string.Empty;
}