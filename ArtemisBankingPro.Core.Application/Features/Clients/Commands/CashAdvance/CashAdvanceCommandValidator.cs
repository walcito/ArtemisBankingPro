using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.CashAdvance;

public class CashAdvanceCommandValidator : AbstractValidator<CashAdvanceCommand>
{
    public CashAdvanceCommandValidator()
    {
        RuleFor(x => x.CreditCardId).GreaterThan(0).WithMessage("Tarjeta inválida.");
        RuleFor(x => x.DestinationAccountNumber).NotEmpty().WithMessage("Debe indicar la cuenta destino.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");
    }
}