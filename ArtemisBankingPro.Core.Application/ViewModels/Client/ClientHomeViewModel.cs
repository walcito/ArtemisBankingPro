namespace ArtemisBankingPro.Core.Application.ViewModels.Client;

public class ClientHomeViewModel
{
    public List<ProductSummaryViewModel> SavingsAccounts { get; set; } = new();
    public List<ProductSummaryViewModel> Loans { get; set; } = new();
    public List<ProductSummaryViewModel> CreditCards { get; set; } = new();
}

public class ProductSummaryViewModel
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public decimal RelevantAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}
