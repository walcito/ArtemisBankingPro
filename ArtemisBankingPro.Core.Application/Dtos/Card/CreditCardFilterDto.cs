using ArtemisBankingPro.Core.Domain.Common.Enums.Card;

namespace ArtemisBankingPro.Core.Application.Dtos.Card
{
    public class CreditCardFilterDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public CardStatus? Status { get; set; }
        public string? Identification { get; set; }
    }
    public class CreditCardPagedResultDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalRecords { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalRecords / PageSize) : 0;
        public List<ListCreditCardDto> Data { get; set; } = new();
    }
        
}
    
