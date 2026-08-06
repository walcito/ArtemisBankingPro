using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.Interfaces.Repositories;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Withdraw;

public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, Unit>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawCommandHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var accounts = await _savingsAccountRepository.GetAllAsync();
        var account = accounts.FirstOrDefault(a => a.AccountNumber == request.AccountNumber);

        if (account is null)
            throw new Exception("La cuenta no existe.");

        if (account.Status != AccountStatus.Active)
            throw new Exception("No se pueden realizar operaciones sobre una cuenta cancelada.");

        if (account.Balance < request.Amount)
            throw new Exception("Fondos insuficientes para realizar el retiro.");

        account.Balance -= request.Amount;
        _savingsAccountRepository.Update(account);

        await _transactionRepository.AddAsync(new Transaction
        {
            SavingsAccountId = account.Id,
            Amount = request.Amount,
            Type = TransactionType.Debit,
            Date = DateTime.UtcNow
        });

        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}