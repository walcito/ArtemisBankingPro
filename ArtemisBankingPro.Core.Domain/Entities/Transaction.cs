using ArtemisBankingPro.Core.Domain.Common.Enums;

namespace ArtemisBankingPro.Core.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Approved;
    public DateTime Date { get; set; }
    public int SavingsAccountId { get; set; }
    public SavingsAccount? SavingsAccount { get; set; }
    public int? PerformedByCashierId { get; set; }
    public User? PerformedByCashier { get; set; }
}