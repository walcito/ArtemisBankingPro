namespace ArtemisBankingPro.Core.Application.Dtos.Commerce;

public class TradeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Rnc { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
