using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayCreditCardFromClient;

public class PayCreditCardFromClientCommandValidator : AbstractValidator<PayCreditCardFromClientCommand>
{
    public PayCreditCardFromClientCommandValidator()
    {
        RuleFor(x => x.SourceAccountNumber).NotEmpty().WithMessage("Debe indicar la cuenta de origen.");
        RuleFor(x => x.CreditCardId).GreaterThan(0).WithMessage("Tarjeta inválida.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto a pagar debe ser mayor a cero.");
    }
}