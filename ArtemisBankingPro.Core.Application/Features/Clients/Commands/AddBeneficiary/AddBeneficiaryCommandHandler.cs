using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
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
            throw new Exception("El número de cuenta ingresado no corresponde a una cuenta válida.");

        if (targetAccount.Status != AccountStatus.Active)
            throw new Exception("No puede agregar una cuenta cancelada como beneficiario.");

        // Regla clave: no puede agregarse a sí mismo como beneficiario
        if (targetAccount.UserId == request.OwnerUserId)
            throw new Exception("No puede agregar una cuenta propia como beneficiario. Utilice la opción Transferencia para mover fondos entre sus cuentas.");

        var existingBeneficiaries = await _beneficiaryRepository.GetAllAsync();
        if (existingBeneficiaries.Any(b => b.OwnerUserId == request.OwnerUserId && b.BeneficiaryAccountNumber == request.BeneficiaryAccountNumber))
            throw new Exception("Esta cuenta ya se encuentra registrada como beneficiario.");

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