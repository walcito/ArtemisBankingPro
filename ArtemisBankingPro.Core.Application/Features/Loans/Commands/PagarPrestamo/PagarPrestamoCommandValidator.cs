using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.PagarPrestamo;

public class PagarPrestamoCommandValidator : AbstractValidator<PagarPrestamoCommand>
{
    public PagarPrestamoCommandValidator()
    {
        RuleFor(x => x.LoanId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar un préstamo válido.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("El monto a pagar debe ser mayor a cero.");

        RuleFor(x => x.SavingsAccountId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar una cuenta de ahorros de origen válida.");
    }
}
