using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using AutoMapper;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Card.Queries.GetCreditCards
{
    public class GetCreditCardByIdQueryHandler : IRequestHandler<GetCreditCardByIdQuery, DetailCreditCardDto>
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IMapper _mapper;
        public GetCreditCardByIdQueryHandler(
            ICreditCardRepository creditCardRepository,
            IMapper mapper)
        {
            _creditCardRepository = creditCardRepository;
            _mapper = mapper;
        }
        public async Task<DetailCreditCardDto> Handle(GetCreditCardByIdQuery request, CancellationToken cancellationToken)
        {
            var card = await _creditCardRepository.GetByIdWithConsumptionsAsync(request.CardId);
            if (card is null)
                throw new KeyNotFoundException("La tarjeta especificada no existe.");
            return _mapper.Map<DetailCreditCardDto>(card);
        }
    }
}
