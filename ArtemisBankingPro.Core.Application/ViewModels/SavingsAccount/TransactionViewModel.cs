using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;

namespace ArtemisBankingPro.Core.Application.ViewModels.SavingsAccount;

public class TransactionViewModel
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
}
