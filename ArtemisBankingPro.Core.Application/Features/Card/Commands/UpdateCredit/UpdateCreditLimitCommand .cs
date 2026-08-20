using ArtemisBankingPro.Core.Application.Dtos.Card;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Card.Commands.UpdateCredit
{
    public class UpdateCreditLimitCommand : IRequest<ListCreditCardDto>
    {
        public int CardId { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
