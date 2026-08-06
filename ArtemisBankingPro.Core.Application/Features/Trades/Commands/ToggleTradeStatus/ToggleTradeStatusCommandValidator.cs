using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Commands.ToggleTradeStatus;

public class ToggleTradeStatusCommandValidator : AbstractValidator<ToggleTradeStatusCommand>
{
    public ToggleTradeStatusCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Comercio inválido.");
    }
}