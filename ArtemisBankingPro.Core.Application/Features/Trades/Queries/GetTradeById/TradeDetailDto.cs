namespace ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTradeById;

public class TradeDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Rnc { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? AssociatedUsername { get; set; }
}