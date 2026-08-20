using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.DesembolsarPrestamo;

public class DesembolsarPrestamoCommandHandler : IRequestHandler<DesembolsarPrestamoCommand, bool>
{
    private readonly IGenericRepository<Loan> _loanRepository;
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;

    public DesembolsarPrestamoCommandHandler(
        IGenericRepository<Loan> loanRepository,
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Transaction> transactionRepository)
    {
        _loanRepository = loanRepository;
        _savingsAccountRepository = savingsAccountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<bool> Handle(DesembolsarPrestamoCommand request, CancellationToken cancellationToken)
    {
        var loan = await _loanRepository.GetByIdAsync(request.LoanId);
        if (loan is null)
            throw new Exception("El préstamo no existe.");

        if (loan.Status != LoanStatus.Active)
            throw new Exception("El préstamo no está activo.");

        // Buscar la cuenta principal del cliente
        var accounts = await _savingsAccountRepository.GetAllAsync();
        var principalAccount = accounts.FirstOrDefault(a =>
            a.UserId == loan.UserId &&
            a.Type == AccountType.Principal &&
            a.Status == AccountStatus.Active);

        if (principalAccount is null)
            throw new Exception("El cliente no tiene una cuenta principal activa.");

        // Acreditar el monto desembolsado a la cuenta principal
        principalAccount.Balance += loan.DisbursedAmount;
        await _savingsAccountRepository.UpdateAsync(principalAccount);

        // Registrar la transacción tipo CRÉDITO
        var transaction = new Transaction
        {
            SavingsAccountId = principalAccount.Id,
            Amount = loan.DisbursedAmount,
            Type = TransactionType.Credit,
            Status = TransactionStatus.Approved,
            Date = DateTime.UtcNow
        };

        await _transactionRepository.AddAsync(transaction);

        return true;
    }
}
