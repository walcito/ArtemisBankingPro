using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.AsignarPrestamo;

public class AsignarPrestamoCommandValidator : AbstractValidator<AsignarPrestamoCommand>
{
    public AsignarPrestamoCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar un cliente válido.");

        RuleFor(x => x.DisbursedAmount)
            .GreaterThan(0)
            .WithMessage("El monto del préstamo debe ser mayor a 0.");

        RuleFor(x => x.AnnualInterestRate)
            .GreaterThanOrEqualTo(0)
            .WithMessage("La tasa de interés no puede ser negativa.");

        RuleFor(x => x.TermInMonths)
            .GreaterThan(0)
            .WithMessage("El plazo debe ser al menos 1 mes.");
    }
}
