using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.PagarPrestamo;

public class PagarPrestamoCommandHandler : IRequestHandler<PagarPrestamoCommand, bool>
{
    private readonly IGenericRepository<Loan> _loanRepository;
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PagarPrestamoCommandHandler(
        IGenericRepository<Loan> loanRepository,
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _savingsAccountRepository = savingsAccountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(PagarPrestamoCommand request, CancellationToken cancellationToken)
    {
        // 1. Obtener el préstamo con sus cuotas
        var loans = await _loanRepository.GetAllListWithIncludeAsync(l => l.Installments);
        var loan = loans.FirstOrDefault(l => l.Id == request.LoanId);

        if (loan is null)
            throw new Exception("El préstamo no existe.");

        if (loan.Status != LoanStatus.Active)
            throw new Exception("El préstamo no está activo.");

        // 2. Obtener la cuenta de ahorros
        var savingsAccount = await _savingsAccountRepository.GetByIdAsync(request.SavingsAccountId);
        if (savingsAccount is null)
            throw new Exception("La cuenta de ahorros seleccionada no existe.");

        if (savingsAccount.Status != AccountStatus.Active)
            throw new Exception("La cuenta de ahorros seleccionada no está activa.");

        if (savingsAccount.Balance < request.Amount)
            throw new Exception("La cuenta de ahorros no tiene balance suficiente.");

        // 3. Validar sobrepago contra la deuda total restante
        var unpaidInstallments = loan.Installments
            .Where(i => i.Status != InstallmentStatus.Paid)
            .OrderBy(i => i.InstallmentNumber)
            .ToList();

        decimal totalRemainingDebt = unpaidInstallments.Sum(i => i.InstallmentAmount - i.PaidAmount);

        if (request.Amount > totalRemainingDebt)
            throw new Exception("El monto a pagar excede la deuda total pendiente del préstamo.");

        // Iniciar transacción de base de datos para asegurar atomicidad (ACID)
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            decimal remainingPayment = request.Amount;

            foreach (var installment in unpaidInstallments)
            {
                if (remainingPayment <= 0)
                    break;

                decimal remainingInstallmentAmount = installment.InstallmentAmount - installment.PaidAmount;
                decimal paymentToInstallment = Math.Min(remainingPayment, remainingInstallmentAmount);

                // Calcular la división del pago entre interés y amortización de capital
                decimal previousPaidAmount = installment.PaidAmount;
                decimal remainingInterestBefore = Math.Max(installment.Interest - previousPaidAmount, 0);

                decimal interestPaidThisTime = Math.Min(paymentToInstallment, remainingInterestBefore);
                decimal principalPaidThisTime = paymentToInstallment - interestPaidThisTime;

                // Actualizar la cuota
                installment.PaidAmount += paymentToInstallment;
                if (installment.PaidAmount >= installment.InstallmentAmount)
                {
                    installment.Status = InstallmentStatus.Paid;
                }
                else
                {
                    installment.Status = InstallmentStatus.PartiallyPaid;
                }

                // Reducir la deuda actual del préstamo con el capital amortizado
                loan.CurrentDebt -= principalPaidThisTime;

                // Reducir el saldo restante del pago
                remainingPayment -= paymentToInstallment;
            }

            // Asegurar que la deuda no sea negativa
            if (loan.CurrentDebt < 0)
                loan.CurrentDebt = 0;

            // Si todas las cuotas del préstamo han sido pagadas, marcar el préstamo como pagado
            if (loan.Installments.All(i => i.Status == InstallmentStatus.Paid))
            {
                loan.Status = LoanStatus.Paid;
            }

            // 4. Actualizar el saldo de la cuenta de ahorros
            savingsAccount.Balance -= request.Amount;

            // 5. Registrar la transacción de débito
            var transaction = new Transaction
            {
                SavingsAccountId = savingsAccount.Id,
                Amount = request.Amount,
                Type = TransactionType.Debit,
                Status = TransactionStatus.Approved,
                Date = DateTime.UtcNow
            };

            // Guardar cambios secuencialmente
            await _savingsAccountRepository.UpdateAsync(savingsAccount);
            await _loanRepository.UpdateAsync(loan);
            await _transactionRepository.AddAsync(transaction);

            // Confirmar transacción
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            // Revertir transacción en caso de error
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
