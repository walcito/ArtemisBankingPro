using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Withdraw;

public class WithdrawCommandValidator : AbstractValidator<WithdrawCommand>
{
    public WithdrawCommandValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .WithMessage("Debe especificar el número de cuenta.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("El monto a retirar debe ser mayor a cero.");
    }
}