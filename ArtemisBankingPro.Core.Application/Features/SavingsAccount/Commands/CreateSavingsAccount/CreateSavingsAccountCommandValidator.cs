using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;

public class CreateSavingsAccountCommandValidator : AbstractValidator<CreateSavingsAccountCommand>
{
    public CreateSavingsAccountCommandValidator()
{
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar un cliente válido.");

        RuleFor(x => x.InitialAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El monto inicial no puede ser negativo.");
    }
}