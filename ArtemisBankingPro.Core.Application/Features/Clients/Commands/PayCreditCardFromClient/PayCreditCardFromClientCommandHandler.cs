using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;
using ArtemisBankingPro.Core.Domain.Entities.Users;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayCreditCardFromClient;

public class PayCreditCardFromClientCommandHandler : IRequestHandler<PayCreditCardFromClientCommand, Unit>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<CreditCard> _creditCardRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IGenericRepository<User> _userRepository;

    public PayCreditCardFromClientCommandHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<CreditCard> creditCardRepository,
        IGenericRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork, IEmailService emailService, IGenericRepository<User> userRepository)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _creditCardRepository = creditCardRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(PayCreditCardFromClientCommand request, CancellationToken cancellationToken)
    {
        var accounts = await _savingsAccountRepository.GetAllAsync();
        var sourceAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.SourceAccountNumber);

        if (sourceAccount is null)
            throw new Exception("La cuenta de origen no existe.");

        if (sourceAccount.UserId != request.RequestingUserId)
            throw new Exception("Solo puede operar sobre cuentas de su propiedad.");

        if (sourceAccount.Status != AccountStatus.Active)
            throw new Exception("La cuenta de origen debe estar activa.");

        var creditCard = await _creditCardRepository.GetByIdAsync(request.CreditCardId);

        if (creditCard is null)
            throw new Exception("La tarjeta de crÃ©dito no existe.");

        if (creditCard.UserId != request.RequestingUserId)
            throw new Exception("Esta tarjeta no le pertenece.");

        if (creditCard.Status != CardStatus.Active)
            throw new Exception("La tarjeta de crÃ©dito debe estar activa.");

        if (sourceAccount.Balance < request.Amount)
            throw new Exception("Fondos insuficientes en la cuenta.");

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

                var user = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (user != null)
        {
            await _emailService.SendEmailAsync(user.Email, "Notificación de Transacción", "Se ha procesado una transacción exitosamente.");
        }

        return Unit.Value;
    }
}
