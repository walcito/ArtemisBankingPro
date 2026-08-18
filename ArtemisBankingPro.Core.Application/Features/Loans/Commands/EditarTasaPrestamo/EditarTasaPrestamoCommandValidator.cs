using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.EditarTasaPrestamo;

public class EditarTasaPrestamoCommandValidator : AbstractValidator<EditarTasaPrestamoCommand>
{
    public EditarTasaPrestamoCommandValidator()
    {
        RuleFor(x => x.LoanId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar un préstamo válido.");

        RuleFor(x => x.NewAnnualInterestRate)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La tasa de interés no puede ser negativa.");
    }
}
