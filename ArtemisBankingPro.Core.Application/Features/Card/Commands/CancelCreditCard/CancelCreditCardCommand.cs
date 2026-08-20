using ArtemisBankingPro.Core.Application.Dtos.Card;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Card.Commands.CancelCreditCard
{
    public class CancelCreditCardCommand : IRequest<ListCreditCardDto>
    {
        public int CardId { get; set; }
        public string? Reason { get; set; }
    }
}
