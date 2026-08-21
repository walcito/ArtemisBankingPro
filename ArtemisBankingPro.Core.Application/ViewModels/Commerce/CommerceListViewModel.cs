namespace ArtemisBankingPro.Core.Application.ViewModels.Commerce;

public class CommerceListViewModel
{
    public List<CommerceViewModel> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
