using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Card.Commands.UpdateCredit
{
    public class UpdateCreditLimitCommandValidator : AbstractValidator<UpdateCreditLimitCommand>
    {
        public UpdateCreditLimitCommandValidator()
        {
            RuleFor(x => x.CardId)
                .GreaterThan(0)
                .WithMessage("Debe especificar una tarjeta válida.");
            RuleFor(x => x.CreditLimit)
                .GreaterThan(0)
                .WithMessage("El nuevo límite de crédito debe ser mayor a RD$0.00.");
        }
    }
}
