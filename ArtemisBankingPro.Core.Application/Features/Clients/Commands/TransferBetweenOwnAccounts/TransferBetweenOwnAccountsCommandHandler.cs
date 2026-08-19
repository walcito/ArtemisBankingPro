using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferBetweenOwnAccounts;

public class TransferBetweenOwnAccountsCommandHandler : IRequestHandler<TransferBetweenOwnAccountsCommand, Unit>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransferBetweenOwnAccountsCommandHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(TransferBetweenOwnAccountsCommand request, CancellationToken cancellationToken)
    {
        var accounts = await _savingsAccountRepository.GetAllAsync();

        var sourceAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.SourceAccountNumber);
        var destinationAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.DestinationAccountNumber);

        if (sourceAccount is null || destinationAccount is null)
            throw new Exception("Una de las cuentas indicadas no existe.");

        // Validación clave: ambas cuentas deben pertenecer al usuario autenticado
        if (sourceAccount.UserId != request.RequestingUserId || destinationAccount.UserId != request.RequestingUserId)
            throw new Exception("Solo puede transferir entre cuentas de su propiedad.");

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