using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Card.Commands.CancelCreditCard
{
    public class CancelCreditCardCommandValidator : AbstractValidator<CancelCreditCardCommand>
    {
        public CancelCreditCardCommandValidator()
    {
        RuleFor(x => x.CardId)
            .GreaterThan(0)
            .WithMessage("Debe especificar una tarjeta válida.");
        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("El motivo de cancelación no puede exceder los 500 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
    }
}
