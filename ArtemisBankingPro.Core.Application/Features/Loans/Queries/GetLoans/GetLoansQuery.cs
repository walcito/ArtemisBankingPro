using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetLoans;

public class GetLoansQuery : IRequest<PagedResult<LoanDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public LoanStatus? Status { get; set; }
    public string? IdentificationNumber { get; set; }
}
