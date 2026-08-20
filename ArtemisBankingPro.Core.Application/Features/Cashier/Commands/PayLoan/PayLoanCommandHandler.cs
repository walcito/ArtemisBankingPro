using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayLoan;

public class PayLoanCommandHandler : IRequestHandler<PayLoanCommand, Unit>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Loan> _loanRepository;
    private readonly IGenericRepository<AmortizationInstallment> _installmentRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayLoanCommandHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Loan> loanRepository,
        IGenericRepository<AmortizationInstallment> installmentRepository,
        IGenericRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _loanRepository = loanRepository;
        _installmentRepository = installmentRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(PayLoanCommand request, CancellationToken cancellationToken)
    {
        var accounts = await _savingsAccountRepository.GetAllAsync();
        var sourceAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.SourceAccountNumber);

        if (sourceAccount is null)
            throw new Exception("La cuenta de origen no existe.");

        if (sourceAccount.Status != AccountStatus.Active)
            throw new Exception("La cuenta de origen debe estar activa.");

        var loan = await _loanRepository.GetByIdAsync(request.LoanId);

        if (loan is null)
            throw new Exception("El préstamo no existe.");

        if (loan.Status == LoanStatus.Paid)
            throw new Exception("Este préstamo ya fue pagado en su totalidad.");

        if (sourceAccount.Balance < request.Amount)
            throw new Exception("Fondos insuficientes en la cuenta.");

        // Regla clave: no permitir sobrepago
        if (request.Amount > loan.CurrentDebt)
            throw new Exception("El monto a pagar no puede ser mayor a la deuda actual del préstamo.");

        var allInstallments = await _installmentRepository.GetAllAsync();

        var pendingInstallments = allInstallments
            .Where(i => i.LoanId == loan.Id && i.Status != InstallmentStatus.Paid)
            .OrderBy(i => i.InstallmentNumber)
            .ToList();

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var remainingAmount = request.Amount;

            foreach (var installment in pendingInstallments)
            {
                if (remainingAmount <= 0)
                    break;

                var pendingOnThisInstallment = installment.InstallmentAmount - installment.PaidAmount;

                if (remainingAmount >= pendingOnThisInstallment)
                {
                    // Cubre esta cuota por completo
                    installment.PaidAmount = installment.InstallmentAmount;
                    installment.Status = InstallmentStatus.Paid;
                    remainingAmount -= pendingOnThisInstallment;
                }
                else
                {
                    // Solo cubre una parte de esta cuota
                    installment.PaidAmount += remainingAmount;
                    installment.Status = InstallmentStatus.PartiallyPaid;
                    remainingAmount = 0;
                }

                _installmentRepository.Update(installment);
            }

            sourceAccount.Balance -= request.Amount;
            loan.CurrentDebt -= request.Amount;

            if (loan.CurrentDebt <= 0)
            {
                loan.CurrentDebt = 0;
                loan.Status = LoanStatus.Paid;
            }

            _savingsAccountRepository.Update(sourceAccount);
            _loanRepository.Update(loan);

            await _transactionRepository.AddAsync(new Transaction
            {
                SavingsAccountId = sourceAccount.Id,
                Amount = request.Amount,
                Type = TransactionType.Debit,
                Status = TransactionStatus.Approved,
                Date = DateTime.UtcNow,
                PerformedByCashierId = request.CashierUserId
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