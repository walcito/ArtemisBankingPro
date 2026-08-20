using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.HermesPay.Commands.ProcessPayment
{
    public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
    {
        public ProcessPaymentCommandValidator()
        {
            RuleFor(x => x.CommerceId)
                .GreaterThan(0)
                .WithMessage("El identificador del comercio es requerido.");
            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .WithMessage("El número de tarjeta es requerido.")
                .Length(16)
                .WithMessage("El número de tarjeta debe contener exactamente 16 dígitos.")
                .Matches(@"^\d{16}$")
                .WithMessage("El número de tarjeta solo puede contener dígitos.");
            RuleFor(x => x.MonthExpirationCard)
                .NotEmpty()
                .WithMessage("El mes de expiración es requerido.")
                .Matches(@"^(0[1-9]|1[0-2])$")
                .WithMessage("El mes de expiración debe tener un valor válido entre 01 y 12.");
            RuleFor(x => x.YearExpirationCard)
                .NotEmpty()
                .WithMessage("El año de expiración es requerido.")
                .Matches(@"^(\d{2}|\d{4})$")
                .WithMessage("El año de expiración no tiene un formato válido.");
            RuleFor(x => x.Cvc)
                .NotEmpty()
                .WithMessage("El CVC es requerido.")
                .Length(3)
                .WithMessage("El CVC debe contener exactamente 3 dígitos.")
                .Matches(@"^\d{3}$")
                .WithMessage("El CVC solo puede contener dígitos.");
            RuleFor(x => x.TransactionAmount)
                .GreaterThan(0)
                .WithMessage("El monto de la transacción debe ser mayor que cero.");
        }
    }
}
