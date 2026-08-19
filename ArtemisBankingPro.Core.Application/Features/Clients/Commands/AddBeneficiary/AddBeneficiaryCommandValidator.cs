using FluentValidation;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.AddBeneficiary;

public class AddBeneficiaryCommandValidator : AbstractValidator<AddBeneficiaryCommand>
{
    public AddBeneficiaryCommandValidator()
    {
        RuleFor(x => x.OwnerUserId).GreaterThan(0).WithMessage("Usuario inválido.");
        RuleFor(x => x.BeneficiaryAccountNumber).NotEmpty().WithMessage("Debe indicar el número de cuenta.");
        RuleFor(x => x.Alias).NotEmpty().WithMessage("Debe indicar un alias para el beneficiario.");
    }
}