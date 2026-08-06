using ArtemisBankingPro.Core.Application.Interfaces.Repositories;
using ArtemisBankingPro.Core.Domain.Entities;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Commands.ToggleTradeStatus;

public class ToggleTradeStatusCommandHandler : IRequestHandler<ToggleTradeStatusCommand, Unit>
{
    private readonly IGenericRepository<Trade> _tradeRepository;
    private readonly IGenericRepository<User> _userRepository;

    public ToggleTradeStatusCommandHandler(
        IGenericRepository<Trade> tradeRepository,
        IGenericRepository<User> userRepository)
    {
        _tradeRepository = tradeRepository;
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(ToggleTradeStatusCommand request, CancellationToken cancellationToken)
    {
        var trade = await _tradeRepository.GetByIdAsync(request.Id);

        if (trade is null)
            throw new Exception("El comercio no existe.");

        if (trade.IsActive)
        {
            // Caso: desactivar el comercio
            trade.IsActive = false;
            _tradeRepository.Update(trade);

            // Inactivar también al usuario asociado, si existe
            if (trade.AssociatedUserId.HasValue)
            {
                var associatedUser = await _userRepository.GetByIdAsync(trade.AssociatedUserId.Value);

                if (associatedUser is not null && associatedUser.IsActive)
                {
                    associatedUser.IsActive = false;
                    _userRepository.Update(associatedUser);
                }
            }
        }
        else
        {
            // Caso: reactivar el comercio
            trade.IsActive = true;
            _tradeRepository.Update(trade);

            // No se reactiva el usuario asociado aquí
            // Eso queda como una acción manual y separada por parte del administrador
        }

        return Unit.Value;
    }
}