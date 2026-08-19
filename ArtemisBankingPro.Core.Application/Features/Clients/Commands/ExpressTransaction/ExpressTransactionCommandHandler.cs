using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;
using ArtemisBankingPro.Core.Domain.Entities.Users;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.ExpressTransaction;

public class ExpressTransactionCommandHandler : IRequestHandler<ExpressTransactionCommand, Unit>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IGenericRepository<User> _userRepository;

    public ExpressTransactionCommandHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork, IEmailService emailService, IGenericRepository<User> userRepository)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(ExpressTransactionCommand request, CancellationToken cancellationToken)
    {
        var accounts = await _savingsAccountRepository.GetAllAsync();

        var sourceAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.SourceAccountNumber);
        var destinationAccount = accounts.FirstOrDefault(a => a.AccountNumber == request.DestinationAccountNumber);

        if (sourceAccount is null)
            throw new Exception("La cuenta de origen no existe.");

        // Validaci√≥n clave: la cuenta de origen debe ser del usuario autenticado
        if (sourceAccount.UserId != request.RequestingUserId)
            throw new Exception("Solo puede operar sobre cuentas de su propiedad.");

        if (destinationAccount is null)
            throw new Exception("La cuenta de destino no existe.");

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

                var user = await _userRepository.GetByIdAsync(request.RequestingUserId);
        if (user != null)
        {
            await _emailService.SendEmailAsync(user.Email, "NotificaciÛn de TransacciÛn", "Se ha procesado una transacciÛn exitosamente.");
        }

        return Unit.Value;
    }
}
