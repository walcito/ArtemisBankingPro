using MediatR;
using ArtemisBankingPro.Core.Application.Common.Models;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetAccountTransactions;

public class GetAccountTransactionsQuery : IRequest<PagedResult<TransactionDto>>
{
    public string AccountNumber { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}