using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTradeById;

public class GetTradeByIdQuery : IRequest<TradeDetailDto>
{
    public int Id { get; set; }
}