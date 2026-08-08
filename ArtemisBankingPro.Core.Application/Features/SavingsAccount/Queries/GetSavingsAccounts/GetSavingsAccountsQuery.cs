using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetSavingsAccounts;

public class GetSavingsAccountsQuery : IRequest<PagedResult<SavingsAccountDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public AccountStatus? Status { get; set; }
    public AccountType? Type { get; set; }
    public string? IdentificationNumber { get; set; }
}
