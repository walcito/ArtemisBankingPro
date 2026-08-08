using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities;
using MediatR;
using SavingsAccount = ArtemisBankingPro.Core.Domain.Entities.SavingsAccount;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetAccountTransactions;

public class GetAccountTransactionsQueryHandler
    : IRequestHandler<GetAccountTransactionsQuery, PagedResult<TransactionDto>>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Transaction> _transactionRepository;

    public GetAccountTransactionsQueryHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Transaction> transactionRepository)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<PagedResult<TransactionDto>> Handle(
        GetAccountTransactionsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _savingsAccountRepository.GetAllAsync();
        var account = accounts.FirstOrDefault(a => a.AccountNumber == request.AccountNumber);

        if (account is null)
            throw new Exception("La cuenta no existe.");

        var transactions = await _transactionRepository.GetAllAsync();

        var filtered = transactions
            .Where(t => t.SavingsAccountId == account.Id)
            .OrderByDescending(t => t.Date);

        var totalCount = filtered.Count();

        var items = filtered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                Date = t.Date
            })
            .ToList();

        return new PagedResult<TransactionDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
