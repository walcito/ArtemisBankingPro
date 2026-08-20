using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;
using ArtemisBankingPro.Core.Domain.Entities.Users;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.CashAdvance;

public class CashAdvanceCommandHandler : IRequestHandler<CashAdvanceCommand, Unit>
{
    private const decimal CashAdvanceInterestRate = 0.0625m;

    private readonly IGenericRepository<CreditCard> _creditCardRepository;
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IGenericRepository<User> _userRepository;

    public CashAdvanceCommandHandler(
        IGenericRepository<CreditCard> creditCardRepository,
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork, IEmailService emailService, IGenericRepository<User> userRepository)
    {
        _creditCardRepository = creditCardRepository;
        _savingsAccountRepository = savingsAccountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(CashAdvanceCommand request, CancellationToken cancellationToken)
    {
        var creditCard = await _creditCardRepository.GetByIdAsync(request.CreditCardId);

        if (creditCard is null)
            throw new Exception("La tarjeta no existe.");

        if (creditCard.UserId != request.RequestingUserId)
            throw new Exception("Esta tarjeta no le pertenece.");

        if (creditCard.Status != CardStatus.Active)
            throw new Exception("La tarjeta debe estar activa.");

        var accounts = await _savingsAccountRepository.GetAllAsync();
        var destinationAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.DestinationAccountNumber);

        if (destinationAccount is null)
            throw new Exception("La cuenta destino no existe.");

        if (destinationAccount.UserId != request.RequestingUserId)
            throw new Exception("Solo puede recibir el avance en una cuenta de su propiedad.");

        if (destinationAccount.Status != AccountStatus.Active)
            throw new Exception("La cuenta destino debe estar activa.");

        // El monto que se carga a la deuda incluye el interÃ©s del avance
        var totalDebtToAdd = request.Amount + (request.Amount * CashAdvanceInterestRate);

        if (totalDebtToAdd > creditCard.AvailableCredit)
            throw new Exception("CrÃ©dito disponible insuficiente para este avance.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            creditCard.CurrentDebt += totalDebtToAdd;
            destinationAccount.Balance += request.Amount;

            _creditCardRepository.Update(creditCard);
            _savingsAccountRepository.Update(destinationAccount);

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

                var user = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (user != null)
        {
            await _emailService.SendEmailAsync(user.Email, "Notificación de Transacción", "Se ha procesado una transacción exitosamente.");
        }

        return Unit.Value;
    }
}
