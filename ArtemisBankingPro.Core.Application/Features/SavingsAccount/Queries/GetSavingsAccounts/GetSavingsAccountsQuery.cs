using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using MediatR;
using ArtemisBankingPro.Core.Application.Dtos.SavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetSavingsAccounts;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetSavingsAccounts;

public class GetSavingsAccountsQuery : IRequest<PagedResult<SavingsAccountDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public AccountStatus? Status { get; set; }
    public AccountType? Type { get; set; }
    public string? IdentificationNumber { get; set; }
}
