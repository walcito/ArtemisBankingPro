using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities;
using MediatR;
using SavingsAccount = ArtemisBankingPro.Core.Domain.Entities.SavingsAccount;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetSavingsAccounts;

public class GetSavingsAccountsQueryHandler
    : IRequestHandler<GetSavingsAccountsQuery, PagedResult<SavingsAccountDto>>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;

    public GetSavingsAccountsQueryHandler(IGenericRepository<SavingsAccount> savingsAccountRepository)
    {
        _savingsAccountRepository = savingsAccountRepository;
    }

    public async Task<PagedResult<SavingsAccountDto>> Handle(
        GetSavingsAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _savingsAccountRepository.GetAllAsync();

        var filtered = accounts.AsQueryable();

        if (request.Status.HasValue)
            filtered = filtered.Where(a => a.Status == request.Status.Value);

        if (request.Type.HasValue)
            filtered = filtered.Where(a => a.Type == request.Type.Value);

        if (!string.IsNullOrWhiteSpace(request.IdentificationNumber))
            filtered = filtered.Where(a => a.User != null &&
                a.User.IdentificationNumber.Contains(request.IdentificationNumber));

        var totalCount = filtered.Count();

        var items = filtered
            .OrderByDescending(a => a.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new SavingsAccountDto
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                Balance = a.Balance,
                Type = a.Type,
                Status = a.Status,
                UserFullName = a.User != null ? $"{a.User.FirstName} {a.User.LastName}" : string.Empty,
                UserIdentificationNumber = a.User != null ? a.User.IdentificationNumber : string.Empty
            })
            .ToList();

        return new PagedResult<SavingsAccountDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}