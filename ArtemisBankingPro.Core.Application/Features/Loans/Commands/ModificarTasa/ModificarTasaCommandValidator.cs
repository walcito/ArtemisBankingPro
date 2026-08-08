using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.ModificarTasa;

public class ModificarTasaCommandValidator : AbstractValidator<ModificarTasaCommand>
{
    public ModificarTasaCommandValidator()
    {
        RuleFor(x => x.LoanId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar un préstamo válido.");

        RuleFor(x => x.NewAnnualInterestRate)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La tasa de interés no puede ser negativa.");
    }
}
