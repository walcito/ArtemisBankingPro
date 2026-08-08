using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Commands.UpdateTrade;

public class UpdateTradeCommandHandler : IRequestHandler<UpdateTradeCommand, Unit>
{
    private readonly IGenericRepository<Trade> _tradeRepository;

    public UpdateTradeCommandHandler(IGenericRepository<Trade> tradeRepository)
    {
        _tradeRepository = tradeRepository;
    }

    public async Task<Unit> Handle(UpdateTradeCommand request, CancellationToken cancellationToken)
    {
        var trade = await _tradeRepository.GetByIdAsync(request.Id);

        if (trade is null)
            throw new Exception("El comercio no existe.");

        var allTrades = await _tradeRepository.GetAllAsync();

        // Validar que el correo no pertenezca a otro comercio (excluyendo el propio)
        if (allTrades.Any(t => t.Mail == request.Mail && t.Id != request.Id))
            throw new Exception("Ya existe otro comercio registrado con este correo.");

        trade.Name = request.Name;
        trade.Description = request.Description;
        trade.Mail = request.Mail;
        trade.Phone = request.Phone;

        _tradeRepository.Update(trade);

        return Unit.Value;
    }
}