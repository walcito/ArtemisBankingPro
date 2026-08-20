using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayLoan;

public class PayLoanCommandValidator : AbstractValidator<PayLoanCommand>
{
    public PayLoanCommandValidator()
    {
        RuleFor(x => x.SourceAccountNumber).NotEmpty().WithMessage("Debe indicar la cuenta de origen.");
        RuleFor(x => x.LoanId).GreaterThan(0).WithMessage("Préstamo inválido.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto a pagar debe ser mayor a cero.");
    }
}