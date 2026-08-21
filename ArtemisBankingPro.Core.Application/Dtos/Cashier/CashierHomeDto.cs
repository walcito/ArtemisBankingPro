namespace ArtemisBankingPro.Core.Application.Dtos.Cashier;

public class CashierHomeDto
{
    public int TotalTransactionsToday { get; set; }
    public decimal TotalAmountProcessedToday { get; set; }
    public int RejectedTransactionsToday { get; set; }
}
