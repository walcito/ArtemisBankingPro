namespace ArtemisBankingPro.Core.Application.ViewModels.SavingsAccount;

public class SavingsAccountListViewModel
{
    public List<SavingsAccountViewModel> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
