using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferToBeneficiary;

public class TransferToBeneficiaryCommandHandler : IRequestHandler<TransferToBeneficiaryCommand, Unit>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Beneficiary> _beneficiaryRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransferToBeneficiaryCommandHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Beneficiary> beneficiaryRepository,
        IGenericRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _beneficiaryRepository = beneficiaryRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(TransferToBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        var beneficiary = await _beneficiaryRepository.GetByIdAsync(request.BeneficiaryId);

        if (beneficiary is null)
            throw new Exception("El beneficiario no existe.");

        if (beneficiary.OwnerUserId != request.RequestingUserId)
            throw new Exception("Este beneficiario no le pertenece.");

        var accounts = await _savingsAccountRepository.GetAllAsync();

        var sourceAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.SourceAccountNumber);
        var destinationAccount = accounts.FirstOrDefault(a => a.AccountNumber == beneficiary.BeneficiaryAccountNumber);

        if (sourceAccount is null)
            throw new Exception("La cuenta de origen no existe.");

        if (sourceAccount.UserId != request.RequestingUserId)
            throw new Exception("Solo puede operar sobre cuentas de su propiedad.");

        if (destinationAccount is null)
            throw new Exception("La cuenta del beneficiario ya no existe.");

        if (sourceAccount.Status != AccountStatus.Active || destinationAccount.Status != AccountStatus.Active)
            throw new Exception("Ambas cuentas deben estar activas.");

        if (sourceAccount.Balance < request.Amount)
            throw new Exception("Fondos insuficientes.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            sourceAccount.Balance -= request.Amount;
            destinationAccount.Balance += request.Amount;

            _savingsAccountRepository.Update(sourceAccount);
            _savingsAccountRepository.Update(destinationAccount);

            await _transactionRepository.AddAsync(new Transaction
            {
                SavingsAccountId = sourceAccount.Id,
                Amount = request.Amount,
                Type = TransactionType.Debit,
                Status = TransactionStatus.Approved,
                Date = DateTime.UtcNow
            });

            await _transactionRepository.AddAsync(new Transaction
            {
                SavingsAccountId = destinationAccount.Id,
                Amount = request.Amount,
                Type = TransactionType.Credit,
                Status = TransactionStatus.Approved,
                Date = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        return Unit.Value;
    }
}