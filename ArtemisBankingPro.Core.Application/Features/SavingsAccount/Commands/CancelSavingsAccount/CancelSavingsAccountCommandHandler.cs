using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using MediatR;
using SavingsAccount = ArtemisBankingPro.Core.Domain.Entities.SavingsAccount;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;

public class CancelSavingsAccountCommandHandler : IRequestHandler<CancelSavingsAccountCommand, Unit>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSavingsAccountCommandHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CancelSavingsAccountCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar la cuenta secundaria a cancelar
        var accounts = await _savingsAccountRepository.GetAllAsync();
        var secondaryAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.AccountNumber);

        if (secondaryAccount is null)
            throw new Exception("La cuenta no existe.");

        if (secondaryAccount.Type != AccountType.Secondary)
            throw new Exception("Solo se pueden cancelar cuentas secundarias.");

        if (secondaryAccount.Status != AccountStatus.Active)
            throw new Exception("La cuenta ya se encuentra cancelada.");

        // 2. Buscar la cuenta principal del mismo usuario
        var principalAccount = accounts.FirstOrDefault(a =>
            a.UserId == secondaryAccount.UserId &&
            a.Type == AccountType.Principal &&
            a.Status == AccountStatus.Active);

        if (principalAccount is null)
            throw new Exception("No se encontró una cuenta principal activa para transferir el balance.");

        // 3. Iniciar la transacción de base de datos (todo o nada)
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var amountToTransfer = secondaryAccount.Balance;

            if (amountToTransfer > 0)
            {
                // Débito en la cuenta secundaria
                await _transactionRepository.AddAsync(new Transaction
                {
                    SavingsAccountId = secondaryAccount.Id,
                    Amount = amountToTransfer,
                    Type = TransactionType.Debit,
                    Date = DateTime.UtcNow
                });

                // Crédito en la cuenta principal
                await _transactionRepository.AddAsync(new Transaction
                {
                    SavingsAccountId = principalAccount.Id,
                    Amount = amountToTransfer,
                    Type = TransactionType.Credit,
                    Date = DateTime.UtcNow
                });

                principalAccount.Balance += amountToTransfer;
                secondaryAccount.Balance = 0;

                _savingsAccountRepository.Update(principalAccount);
            }

            secondaryAccount.Status = AccountStatus.Cancelled;
            _savingsAccountRepository.Update(secondaryAccount);

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
