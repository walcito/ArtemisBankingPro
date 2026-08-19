using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferToBeneficiary;

public class TransferToBeneficiaryCommandValidator : AbstractValidator<TransferToBeneficiaryCommand>
{
    public TransferToBeneficiaryCommandValidator()
    {
        RuleFor(x => x.SourceAccountNumber).NotEmpty().WithMessage("Debe indicar la cuenta de origen.");
        RuleFor(x => x.BeneficiaryId).GreaterThan(0).WithMessage("Debe seleccionar un beneficiario.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");
    }
}