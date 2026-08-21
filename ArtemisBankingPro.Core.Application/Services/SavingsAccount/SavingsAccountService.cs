using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Application.Dtos.SavingsAccount;
using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetAccountTransactions;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetSavingsAccounts;
using ArtemisBankingPro.Core.Application.Features.Users;
using ArtemisBankingPro.Core.Application.Interfaces.SavingsAccount;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Services.SavingsAccount;

public class SavingsAccountService : ISavingsAccountService
{
    private readonly IMediator _mediator;

    public SavingsAccountService(IMediator mediator)
    {
        _mediator = mediator;
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
        var users = await _mediator.Send(new GetUsersQuery());
        var clients = users.Where(u => u.Role == "Cliente" && u.IsActive).ToList();

        if (!string.IsNullOrEmpty(identificationNumber))
        {
            clients = clients.Where(c => c.Identification.Contains(identificationNumber)).ToList();
        }

        return clients;
    }
}
