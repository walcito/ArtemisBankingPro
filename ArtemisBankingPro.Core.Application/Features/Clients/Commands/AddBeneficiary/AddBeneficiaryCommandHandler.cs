using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.AddBeneficiary;

public class AddBeneficiaryCommandHandler : IRequestHandler<AddBeneficiaryCommand, int>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Beneficiary> _beneficiaryRepository;

    public AddBeneficiaryCommandHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Beneficiary> beneficiaryRepository)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _beneficiaryRepository = beneficiaryRepository;
    }

    public async Task<int> Handle(AddBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        var accounts = await _savingsAccountRepository.GetAllAsync();
        var targetAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.BeneficiaryAccountNumber);

        if (targetAccount is null)
            throw new Exception("La cuenta indicada no existe.");

        if (targetAccount.Status != AccountStatus.Active)
            throw new Exception("Solo se pueden agregar cuentas activas como beneficiarios.");

        // Regla clave: no puede agregarse a sí mismo como beneficiario
        if (targetAccount.UserId == request.OwnerUserId)
            throw new Exception("No puede agregarse a sí mismo como beneficiario.");

        var newBeneficiary = new Beneficiary
        {
            OwnerUserId = request.OwnerUserId,
            BeneficiaryAccountNumber = request.BeneficiaryAccountNumber,
            Alias = request.Alias
        };

        await _beneficiaryRepository.AddAsync(newBeneficiary);

        return newBeneficiary.Id;
    }
}