using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities.Trades;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTrades;

public class GetTradesQueryHandler : IRequestHandler<GetTradesQuery, PagedResult<TradeDto>>
{
    private readonly IGenericRepository<Trade> _tradeRepository;

    public GetTradesQueryHandler(IGenericRepository<Trade> tradeRepository)
    {
        _tradeRepository = tradeRepository;
    }

    public async Task<PagedResult<TradeDto>> Handle(GetTradesQuery request, CancellationToken cancellationToken)
    {
        var trades = await _tradeRepository.GetAllAsync();

        var filtered = trades.AsQueryable();

        if (request.IsActive.HasValue)
            filtered = filtered.Where(t => t.IsActive == request.IsActive.Value);

        var totalCount = filtered.Count();

        var items = filtered
            .OrderByDescending(t => t.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TradeDto
            {
                Id = t.Id,
                Name = t.Name,
                Mail = t.Mail,
                Phone = t.Phone,
                Rnc = t.Rnc,
                IsActive = t.IsActive
            })
            .ToList();

        return new PagedResult<TradeDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}