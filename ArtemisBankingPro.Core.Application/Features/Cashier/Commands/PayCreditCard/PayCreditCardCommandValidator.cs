using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayCreditCard;

public class PayCreditCardCommandValidator : AbstractValidator<PayCreditCardCommand>
{
    public PayCreditCardCommandValidator()
    {
        RuleFor(x => x.SourceAccountNumber).NotEmpty().WithMessage("Debe indicar la cuenta de origen.");
        RuleFor(x => x.CreditCardId).GreaterThan(0).WithMessage("Tarjeta inválida.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto a pagar debe ser mayor a cero.");
    }
}