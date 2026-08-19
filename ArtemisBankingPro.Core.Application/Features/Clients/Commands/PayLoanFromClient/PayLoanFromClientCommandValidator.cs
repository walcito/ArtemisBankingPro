using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayLoanFromClient;

public class PayLoanFromClientCommandValidator : AbstractValidator<PayLoanFromClientCommand>
{
    public PayLoanFromClientCommandValidator()
    {
        RuleFor(x => x.SourceAccountNumber).NotEmpty().WithMessage("Debe indicar la cuenta de origen.");
        RuleFor(x => x.LoanId).GreaterThan(0).WithMessage("Préstamo inválido.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto a pagar debe ser mayor a cero.");
    }
}