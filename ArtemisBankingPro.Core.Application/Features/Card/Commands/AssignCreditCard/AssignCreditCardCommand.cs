using ArtemisBankingPro.Core.Application.Dtos.Card;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Card.Commands.AssignCreditCard
{
    public class AssignCreditCardCommand : IRequest<CreateCreditCardResponseDto>
    {
        public int ClientId { get; set; }
        public decimal CreditLimit { get; set; }
        public string? Description { get; set; }
    }
}
