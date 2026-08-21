using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Users;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Users;

public class CreateUserCommand : IRequest<UserDto>
{
    public CreateUserDto User { get; set; } = new();
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserService _userService;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;

    public CreateUserCommandHandler(
        IUserService userService,
        IGenericRepository<User> userRepository,
        IGenericRepository<SavingsAccount> savingsAccountRepository)
    {
        _userService = userService;
        _userRepository = userRepository;
        _savingsAccountRepository = savingsAccountRepository;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(request.User);

        // Sincronizar en DbContext de negocio
        var existingUsers = await _userRepository.GetAllAsync();
        var domainUser = existingUsers.FirstOrDefault(u => u.IdentificationNumber == request.User.Identification);

        if (domainUser == null)
        {
            var isClientRole = request.User.Role == "Client" || request.User.Role == "Cliente";
            domainUser = new User
            {
                FirstName = request.User.FirstName,
                LastName = request.User.LastName,
                IdentificationNumber = request.User.Identification,
                Email = request.User.Email,
                Username = request.User.UserName,
                Role = isClientRole ? Roles.Client : Roles.Administrator,
                IsActive = true
            };
            domainUser = await _userRepository.AddAsync(domainUser);
        }

        // Si es cliente, crear automáticamente su cuenta principal con el balance inicial (PDF pág. 23)
        if (request.User.Role == "Client" || request.User.Role == "Cliente")
        {
            var accounts = await _savingsAccountRepository.GetAllAsync();
            var hasPrincipal = accounts.Any(a => a.UserId == domainUser.Id && a.Type == AccountType.Principal);

            if (!hasPrincipal)
            {
                string accountNumber;
                var random = new Random();
                do
                {
                    accountNumber = random.Next(100000000, 999999999).ToString();
                }
                while (accounts.Any(a => a.AccountNumber == accountNumber));

                var principalAccount = new SavingsAccount
                {
                    AccountNumber = accountNumber,
                    Balance = request.User.InitialAmount,
                    Type = AccountType.Principal,
                    Status = AccountStatus.Active,
                    UserId = domainUser.Id
                };
                await _savingsAccountRepository.AddAsync(principalAccount);
            }
        }

        return result;
    }
}