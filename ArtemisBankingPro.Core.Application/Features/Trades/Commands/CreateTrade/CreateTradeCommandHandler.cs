using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Commands.CreateTrade;

public class CreateTradeCommandHandler : IRequestHandler<CreateTradeCommand, int>
{
    private readonly IGenericRepository<Trade> _tradeRepository;

    public CreateTradeCommandHandler(IGenericRepository<Trade> tradeRepository)
    {
        _tradeRepository = tradeRepository;
    }

    public async Task<int> Handle(CreateTradeCommand request, CancellationToken cancellationToken)
    {
        var trades = await _tradeRepository.GetAllAsync();

        // Validar que el RNC no exista ya en otro comercio
        if (trades.Any(t => t.Rnc == request.Rnc))
            throw new Exception("Ya existe un comercio registrado con este RNC.");

        // Validar que el correo no exista ya en otro comercio
        if (trades.Any(t => t.Mail == request.Mail))
            throw new Exception("Ya existe un comercio registrado con este correo.");

        var newTrade = new Trade
        {
            Name = request.Name,
            Description = request.Description,
            Mail = request.Mail,
            Phone = request.Phone,
            Rnc = request.Rnc,
            IsActive = true
        };

        await _tradeRepository.AddAsync(newTrade);

        return newTrade.Id;
    }
}