using MediatR;
using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Application.Dtos.Commerce;
using ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTrades;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTrades;

public class GetTradesQuery : IRequest<PagedResult<TradeDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? IsActive { get; set; }
}