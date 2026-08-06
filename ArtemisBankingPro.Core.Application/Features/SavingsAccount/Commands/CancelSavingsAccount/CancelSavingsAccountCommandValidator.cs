using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;

public class CancelSavingsAccountCommandValidator : AbstractValidator<CancelSavingsAccountCommand>
{
    public CancelSavingsAccountCommandValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .WithMessage("Debe especificar el número de cuenta a cancelar.");
    }
}