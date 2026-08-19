namespace ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetClientHome;

public class ClientHomeDto
{
    public List<ProductSummaryDto> SavingsAccounts { get; set; } = new();
    public List<ProductSummaryDto> Loans { get; set; } = new();
    public List<ProductSummaryDto> CreditCards { get; set; } = new();
}

public class ProductSummaryDto
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty; // número de cuenta, de préstamo, o de tarjeta enmascarado
    public decimal RelevantAmount { get; set; } // balance, deuda actual, o deuda actual según el producto
    public string Status { get; set; } = string.Empty;
}