using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Commands.ThirdPartyTransaction;

public class ThirdPartyTransactionCommandValidator : AbstractValidator<ThirdPartyTransactionCommand>
{
    public ThirdPartyTransactionCommandValidator()
    {
        RuleFor(x => x.SourceAccountNumber).NotEmpty().WithMessage("Debe indicar la cuenta de origen.");
        RuleFor(x => x.DestinationAccountNumber).NotEmpty().WithMessage("Debe indicar la cuenta de destino.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("El monto debe ser mayor a cero.");

        RuleFor(x => x)
            .Must(x => x.SourceAccountNumber != x.DestinationAccountNumber)
            .WithMessage("La cuenta de origen y destino no pueden ser la misma.");
    }
}