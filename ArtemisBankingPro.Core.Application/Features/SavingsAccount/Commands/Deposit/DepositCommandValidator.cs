using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Deposit;

public class DepositCommandValidator : AbstractValidator<DepositCommand>
{
    public DepositCommandValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .WithMessage("Debe especificar el número de cuenta.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("El monto a depositar debe ser mayor a cero.");
    }
}