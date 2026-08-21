namespace ArtemisBankingPro.Core.Application.ViewModels.Cashier;

public class CashierHomeViewModel
{
    public int TotalTransactionsToday { get; set; }
    public decimal TotalAmountProcessedToday { get; set; }
    public int RejectedTransactionsToday { get; set; }
}
