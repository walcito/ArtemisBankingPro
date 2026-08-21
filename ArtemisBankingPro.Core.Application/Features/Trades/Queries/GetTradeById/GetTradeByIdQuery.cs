using MediatR;
using ArtemisBankingPro.Core.Application.Dtos.Commerce;
using ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTradeById;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTradeById;

public class GetTradeByIdQuery : IRequest<TradeDetailDto>
{
    public int Id { get; set; }
}