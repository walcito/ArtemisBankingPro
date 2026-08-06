using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Trades.Commands.CreateTrade;

public class CreateTradeCommandValidator : AbstractValidator<CreateTradeCommand>
{
    public CreateTradeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre del comercio es requerido.");

        RuleFor(x => x.Mail)
            .NotEmpty().WithMessage("El correo es requerido.")
            .EmailAddress().WithMessage("El correo no tiene un formato válido.");

        RuleFor(x => x.Rnc)
            .NotEmpty().WithMessage("El RNC es requerido.");

        RuleFor(x => x.Phone).NotEmpty().WithMessage("El teléfono es requerido.");
    }
}