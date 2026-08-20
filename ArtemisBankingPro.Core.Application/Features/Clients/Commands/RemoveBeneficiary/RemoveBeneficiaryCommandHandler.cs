using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.RemoveBeneficiary;

public class RemoveBeneficiaryCommandHandler : IRequestHandler<RemoveBeneficiaryCommand, Unit>
{
    private readonly IGenericRepository<Beneficiary> _beneficiaryRepository;

    public RemoveBeneficiaryCommandHandler(IGenericRepository<Beneficiary> beneficiaryRepository)
    {
        _beneficiaryRepository = beneficiaryRepository;
    }

    public async Task<Unit> Handle(RemoveBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        var beneficiary = await _beneficiaryRepository.GetByIdAsync(request.Id);

        if (beneficiary is null)
            throw new Exception("El beneficiario no existe.");

        if (beneficiary.OwnerUserId != request.OwnerUserId)
            throw new Exception("No tiene permiso para eliminar este beneficiario.");

        await _beneficiaryRepository.DeleteAsync(beneficiary);

        return Unit.Value;
    }
}