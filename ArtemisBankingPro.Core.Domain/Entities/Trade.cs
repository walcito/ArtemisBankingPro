namespace ArtemisBankingPro.Core.Domain.Entities;

public class Trade
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Rnc { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int? AssociatedUserId { get; set; }
    public User? AssociatedUser { get; set; }
}