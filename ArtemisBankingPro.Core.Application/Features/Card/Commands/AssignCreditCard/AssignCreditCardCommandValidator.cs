using FluentValidation;


namespace ArtemisBankingPro.Core.Application.Features.Card.Commands.AssignCreditCard
{
    public class AssignCreditCardCommandValidator : AbstractValidator<AssignCreditCardCommand>
    {
        public AssignCreditCardCommandValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty()
                .WithMessage("Debe especificar un cliente válido.");
            RuleFor(x => x.CreditLimit)
                .GreaterThan(0)
                .WithMessage("El límite de crédito debe ser mayor a RD$0.00.");
        }
    }
}
