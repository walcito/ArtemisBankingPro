namespace ArtemisBankingPro.Core.Application.ViewModels.SavingsAccount;

public class AccountDetailsViewModel
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public List<TransactionViewModel> Transactions { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
