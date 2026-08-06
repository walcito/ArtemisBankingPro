using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Commands.ToggleTradeStatus;

public class ToggleTradeStatusCommand : IRequest<Unit>
{
    public int Id { get; set; }
}