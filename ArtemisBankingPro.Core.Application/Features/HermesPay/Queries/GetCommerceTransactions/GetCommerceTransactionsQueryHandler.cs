using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.HermesPay.Queries.GetCommerceTransactions
{
    public class GetCommerceTransactionsQueryHandler : IRequestHandler<GetCommerceTransactionsQuery, CommerceTransactionsResultDto>
    {
        private readonly ICardConsumptionRepository _consumptionRepository;
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IGenericRepository<Trade> _tradeRepository;
        public GetCommerceTransactionsQueryHandler(
            ICardConsumptionRepository consumptionRepository,
            ICreditCardRepository creditCardRepository,
            IGenericRepository<Trade> tradeRepository)
        {
            _consumptionRepository = consumptionRepository;
            _creditCardRepository = creditCardRepository;
            _tradeRepository = tradeRepository;
        }
        public async Task<CommerceTransactionsResultDto> Handle(GetCommerceTransactionsQuery request, CancellationToken cancellationToken)
        {
            var commerce = await _tradeRepository.GetByIdAsync(request.CommerceId);
            if (commerce is null)
                throw new KeyNotFoundException("El comercio indicado no existe.");
            var allConsumptions = await _consumptionRepository.GetByCommerceIdAsync(request.CommerceId);
            var query = allConsumptions.AsQueryable();
            if (request.Status.HasValue)
                query = query.Where(c => c.Status == request.Status.Value);
            var totalRecords = query.Count();
            var paged = query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            var items = new List<CommerceTransactionItemDto>();
            foreach (var consumption in paged)
            {
                var card = await _creditCardRepository.GetByIdAsync(consumption.CardId);
                var last4 = card != null && card.CardNumber.Length >= 4 ? card.CardNumber[^4..] : "****";
                items.Add(new CommerceTransactionItemDto
                {
                    Id = consumption.Id,
                    TransactionDate = consumption.TransactionDate,
                    Amount = consumption.Amount,
                    CardLastFourDigits = last4,
                    Status = consumption.Status == ConsumptionStatus.Approved ? "APROBADO" : "RECHAZADO"
                });
            }
            return new CommerceTransactionsResultDto
            {
                CommerceId = request.CommerceId,
                CommerceName = commerce.Name,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalRecords = totalRecords,
                Data = items
            };
        }
    }
}
