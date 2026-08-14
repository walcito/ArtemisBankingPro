using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.HermesPay.Queries.GetCommerceTransactions
{
    public class GetCommerceTransactionsQuery : IRequest<CommerceTransactionsResultDto>
    {
        public int CommerceId { get; set; }
        public ConsumptionStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
    public class CommerceTransactionsResultDto
    {
        public int CommerceId { get; set; }
        public string CommerceName { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalRecords { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalRecords / PageSize) : 0;
        public List<CommerceTransactionItemDto> Data { get; set; } = new();
    }
    public class CommerceTransactionItemDto
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string CardLastFourDigits { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
