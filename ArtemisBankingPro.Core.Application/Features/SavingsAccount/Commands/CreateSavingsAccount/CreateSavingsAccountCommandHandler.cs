using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;

public class CreateSavingsAccountCommandHandler : IRequestHandler<CreateSavingsAccountCommand, int>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;

    public CreateSavingsAccountCommandHandler(
        IGenericRepository<User> userRepository,
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Transaction> transactionRepository)
    {
        _userRepository = userRepository;
        _savingsAccountRepository = savingsAccountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<int> Handle(CreateSavingsAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null || !user.IsActive)
            throw new Exception("El cliente no existe o no está activo.");

        var accounts = await _savingsAccountRepository.GetAllAsync();
        var userAccounts = accounts.Where(a => a.UserId == request.UserId).ToList();

        var accountType = AccountType.Secondary;
        if (!userAccounts.Any(a => a.Type == AccountType.Principal && a.Status == AccountStatus.Active))
        {
            // Si el cliente no tiene una cuenta principal activa previa, la primera se crea como Principal
            accountType = AccountType.Principal;
        }

        // Genera número de cuenta único de 9 dígitos
        string accountNumber;
        do
        {
            accountNumber = GenerateNineDigitNumber();
        }
        while (accounts.Any(a => a.AccountNumber == accountNumber));

        var newAccount = new SavingsAccount
        {
            AccountNumber = accountNumber,
            Balance = request.InitialAmount,
            Type = accountType,
            Status = AccountStatus.Active,
            UserId = request.UserId
        };

        await _savingsAccountRepository.AddAsync(newAccount);

        // 5. Si hay monto inicial, registrar la transacción de crédito
        if (request.InitialAmount > 0)
        {
            var transaction = new Transaction
            {
                SavingsAccountId = newAccount.Id,
                Amount = request.InitialAmount,
                Type = TransactionType.Credit,
                Date = DateTime.UtcNow
            };

            await _transactionRepository.AddAsync(transaction);
        }

        return newAccount.Id;
    }

    private static string GenerateNineDigitNumber()
    {
        var random = new Random();
        return random.Next(100_000_000, 999_999_999).ToString();
    }
}
