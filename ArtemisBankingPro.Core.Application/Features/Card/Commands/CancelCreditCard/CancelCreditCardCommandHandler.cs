using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using AutoMapper;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Card.Commands.CancelCreditCard
{
    public class CancelCreditCardCommandHandler : IRequestHandler<CancelCreditCardCommand, ListCreditCardDto>
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IMapper _mapper;

        public CancelCreditCardCommandHandler(
            ICreditCardRepository creditCardRepository,
            IMapper mapper)
        {
            _creditCardRepository = creditCardRepository;
            _mapper = mapper;
        }

        public async Task<ListCreditCardDto> Handle(CancelCreditCardCommand request, CancellationToken cancellationToken)
        {
            var card = await _creditCardRepository.GetByIdAsync(request.CardId);
            if (card is null)
                throw new KeyNotFoundException("La tarjeta seleccionada no existe.");

            if (card.Status == CardStatus.Cancelled)
                throw new Exception("La tarjeta ya se encuentra cancelada.");

            if (card.CurrentDebt > 0)
                throw new Exception("Para cancelar esta tarjeta, el cliente debe saldar la totalidad de la deuda pendiente.");

            card.Status = CardStatus.Cancelled;
            card.CancelledAt = DateTime.UtcNow;
            card.ModifiedAt = DateTime.UtcNow;

            await _creditCardRepository.UpdateAsync(card);

            return _mapper.Map<ListCreditCardDto>(card);
        }
    }
}