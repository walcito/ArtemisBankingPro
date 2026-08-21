using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Application.Dtos.SavingsAccount;
using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetAccountTransactions;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetSavingsAccounts;
using ArtemisBankingPro.Core.Application.Interfaces.SavingsAccount;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Users;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Services.SavingsAccount;

public class SavingsAccountService : ISavingsAccountService
{
    private readonly IMediator _mediator;
    private readonly IGenericRepository<User> _userRepository;

    public SavingsAccountService(IMediator mediator, IGenericRepository<User> userRepository)
    {
        _mediator = mediator;
        _userRepository = userRepository;
    }

    public async Task<PagedResult<SavingsAccountDto>> GetSavingsAccountsAsync(int pageNumber, int pageSize, string? identificationNumber, AccountStatus? status, AccountType? type)
    {
        return await _mediator.Send(new GetSavingsAccountsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IdentificationNumber = identificationNumber,
            Status = status,
            Type = type
        });
    }

    public async Task<PagedResult<TransactionDto>> GetAccountTransactionsAsync(string accountNumber, int pageNumber, int pageSize)
    {
        return await _mediator.Send(new GetAccountTransactionsQuery
        {
            AccountNumber = accountNumber,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    public async Task CreateSavingsAccountAsync(int userId, decimal initialAmount)
    {
        await _mediator.Send(new CreateSavingsAccountCommand
        {
            UserId = userId,
            InitialAmount = initialAmount
        });
    }

    public async Task CancelSavingsAccountAsync(string accountNumber)
    {
        await _mediator.Send(new CancelSavingsAccountCommand { AccountNumber = accountNumber });
    }

    public async Task<List<UserDto>> GetActiveClientsAsync(string? identificationNumber = null)
    {
        var dbUsers = await _userRepository.GetAllAsync();
        var clients = dbUsers
            .Where(u => u.Role == Roles.Client && u.IsActive)
            .Select(u => new UserDto
            {
                Id = u.Id.ToString(),
                FirstName = u.FirstName,
                LastName = u.LastName,
                Identification = u.IdentificationNumber,
                Email = u.Email,
                UserName = u.Username,
                Role = "Client",
                IsActive = u.IsActive
            })
            .ToList();

        if (!string.IsNullOrEmpty(identificationNumber))
        {
            var searchClean = identificationNumber.Trim().Replace("-", "");
            clients = clients.Where(c => c.Identification.Replace("-", "").Contains(searchClean)).ToList();
        }

        return clients;
    }
}
