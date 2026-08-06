using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Commands.UpdateTrade;

public class UpdateTradeCommandValidator : AbstractValidator<UpdateTradeCommand>
{
    public UpdateTradeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Comercio inválido.");

        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre del comercio es requerido.");

        RuleFor(x => x.Mail)
            .NotEmpty().WithMessage("El correo es requerido.")
            .EmailAddress().WithMessage("El correo no tiene un formato válido.");

        RuleFor(x => x.Phone).NotEmpty().WithMessage("El teléfono es requerido.");
    }
}