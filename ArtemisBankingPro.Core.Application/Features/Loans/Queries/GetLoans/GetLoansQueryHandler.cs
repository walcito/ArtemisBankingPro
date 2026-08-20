using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using MediatR;
using System.Linq;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetLoans;

public class GetLoansQueryHandler : IRequestHandler<GetLoansQuery, PagedResult<LoanDto>>
{
    private readonly IGenericRepository<Loan> _loanRepository;

    public GetLoansQueryHandler(IGenericRepository<Loan> loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<PagedResult<LoanDto>> Handle(
        GetLoansQuery request, CancellationToken cancellationToken)
    {
        var loans = await _loanRepository.GetAllAsync();
        var filtered = loans.AsQueryable();

        if (request.Status.HasValue)
            filtered = filtered.Where(l => l.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(request.IdentificationNumber))
            filtered = filtered.Where(l => l.User != null &&
                l.User.IdentificationNumber.Contains(request.IdentificationNumber));

        var totalCount = filtered.Count();

        var items = filtered
            .OrderByDescending(l => l.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new LoanDto
            {
                Id = l.Id,
                LoanNumber = l.LoanNumber,
                DisbursedAmount = l.DisbursedAmount,
                AnnualInterestRate = l.AnnualInterestRate,
                TermInMonths = l.TermInMonths,
                CurrentDebt = l.CurrentDebt,
                Status = l.Status,
                UserFullName = l.User != null ? $"{l.User.FirstName} {l.User.LastName}" : string.Empty,
                UserIdentificationNumber = l.User != null ? l.User.IdentificationNumber : string.Empty
            })
            .ToList();

        return new PagedResult<LoanDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
