namespace ArtemisBankingPro.Core.Application.ViewModels.Admin;

public class DashboardViewModel
{
    public int TotalActiveUsers { get; set; }
    public int TotalActiveAccounts { get; set; }
    public int TotalActiveLoans { get; set; }
    public int TotalActiveCreditCards { get; set; }
    public decimal TotalDepositsToday { get; set; }
    public decimal TotalTransactionsToday { get; set; }
}
