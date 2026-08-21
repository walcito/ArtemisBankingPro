namespace ArtemisBankingPro.Core.Application.ViewModels.Commerce;

public class CommerceViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Rnc { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
