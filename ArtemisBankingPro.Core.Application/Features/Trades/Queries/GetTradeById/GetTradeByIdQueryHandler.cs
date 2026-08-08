using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTradeById;

public class GetTradeByIdQueryHandler : IRequestHandler<GetTradeByIdQuery, TradeDetailDto>
{
    private readonly IGenericRepository<Trade> _tradeRepository;
    private readonly IGenericRepository<User> _userRepository;

    public GetTradeByIdQueryHandler(
        IGenericRepository<Trade> tradeRepository,
        IGenericRepository<User> userRepository)
    {
        _tradeRepository = tradeRepository;
        _userRepository = userRepository;
    }

    public async Task<TradeDetailDto> Handle(GetTradeByIdQuery request, CancellationToken cancellationToken)
    {
        var trade = await _tradeRepository.GetByIdAsync(request.Id);

        if (trade is null)
            throw new Exception("El comercio no existe.");

        string? username = null;

        if (trade.AssociatedUserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(trade.AssociatedUserId.Value);
            username = user?.Username;
        }

        return new TradeDetailDto
        {
            Id = trade.Id,
            Name = trade.Name,
            Description = trade.Description,
            Mail = trade.Mail,
            Phone = trade.Phone,
            Rnc = trade.Rnc,
            IsActive = trade.IsActive,
            AssociatedUsername = username
        };
    }
}