using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayCreditCard;

public class PayCreditCardCommandHandler : IRequestHandler<PayCreditCardCommand, Unit>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<CreditCard> _creditCardRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayCreditCardCommandHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<CreditCard> creditCardRepository,
        IGenericRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _creditCardRepository = creditCardRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(PayCreditCardCommand request, CancellationToken cancellationToken)
    {
        var accounts = await _savingsAccountRepository.GetAllAsync();
        var sourceAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.SourceAccountNumber);

        if (sourceAccount is null)
            throw new Exception("La cuenta de origen no existe.");

        if (sourceAccount.Status != AccountStatus.Active)
            throw new Exception("La cuenta de origen debe estar activa.");

        var creditCard = await _creditCardRepository.GetByIdAsync(request.CreditCardId);

        if (creditCard is null)
            throw new Exception("La tarjeta de crédito no existe.");

        if (creditCard.Status != CardStatus.Active)
            throw new Exception("La tarjeta de crédito debe estar activa.");

        if (sourceAccount.Balance < request.Amount)
            throw new Exception("Fondos insuficientes en la cuenta.");

        // Regla clave: no permitir sobrepago
        if (request.Amount > creditCard.CurrentDebt)
            throw new Exception("El monto a pagar no puede ser mayor a la deuda actual de la tarjeta.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            sourceAccount.Balance -= request.Amount;
            creditCard.CurrentDebt -= request.Amount;
            creditCard.ModifiedAt = DateTime.UtcNow;

            _savingsAccountRepository.Update(sourceAccount);
            _creditCardRepository.Update(creditCard);

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