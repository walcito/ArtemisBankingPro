namespace ArtemisBankingPro.Core.Application.Features.Cashier.Queries.GetCashierHome;

public class CashierHomeDto
{
    public int TotalTransactionsToday { get; set; }
    public decimal TotalAmountProcessedToday { get; set; }
    public int RejectedTransactionsToday { get; set; }
}