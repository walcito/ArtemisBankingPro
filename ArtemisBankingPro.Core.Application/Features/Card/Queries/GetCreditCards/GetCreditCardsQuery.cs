using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Card.Queries.GetCreditCards
{
    public class GetCreditCardsQuery : IRequest<CreditCardPagedResultDto>
    {
        public CardStatus? Status { get; set; }
        public string? Identification { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
