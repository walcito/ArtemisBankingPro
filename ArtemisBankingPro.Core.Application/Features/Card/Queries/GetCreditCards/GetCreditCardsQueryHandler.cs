using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using AutoMapper;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Card.Queries.GetCreditCards
{
    public class GetCreditCardsQueryHandler : IRequestHandler<GetCreditCardsQuery, CreditCardPagedResultDto>
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IMapper _mapper;
        public GetCreditCardsQueryHandler(
            ICreditCardRepository creditCardRepository,
            IMapper mapper)
        {
            _creditCardRepository = creditCardRepository;
            _mapper = mapper;
        }
        public async Task<CreditCardPagedResultDto> Handle(GetCreditCardsQuery request, CancellationToken cancellationToken)
        {
            var allCards = await _creditCardRepository.GetAllWithUserAsync();
            var query = allCards.AsQueryable();
            if (request.Status.HasValue)
                query = query.Where(c => c.Status == request.Status.Value);
            if (!string.IsNullOrWhiteSpace(request.Identification))
                query = query.Where(c => c.User != null && c.User.IdentificationNumber == request.Identification.Trim());
            var totalRecords = query.Count();
            var paged = query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            return new CreditCardPagedResultDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = totalRecords,
                Data = _mapper.Map<List<ListCreditCardDto>>(paged)
            };
        }
    }
}

