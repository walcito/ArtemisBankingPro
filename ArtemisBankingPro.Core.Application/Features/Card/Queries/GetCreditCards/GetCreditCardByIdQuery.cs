using ArtemisBankingPro.Core.Application.Dtos.Card;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Card.Queries.GetCreditCards
{
    public class GetCreditCardByIdQuery : IRequest<DetailCreditCardDto>
    {
        public int CardId { get; set; }
    }
}
