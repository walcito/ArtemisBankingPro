using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Application.Features.Loans.Commands.AsignarPrestamo;
using ArtemisBankingPro.Core.Application.Features.Loans.Commands.EditarTasaPrestamo;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetEligibleClients;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetLoans;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.ObtenerDetallePrestamo;
using ArtemisBankingPro.Core.Application.Interfaces.Loan;
using ArtemisBankingPro.Core.Domain.Common.Enums.Loans;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Services.Loan;

public class LoanService : ILoanService
{
    private readonly IMediator _mediator;

    public LoanService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<PagedResult<LoanDto>> GetLoansAsync(int pageNumber, int pageSize, string? identificationNumber, LoanStatus? status)
    {
        return await _mediator.Send(new GetLoansQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IdentificationNumber = identificationNumber,
            Status = status
        });
    }

    public async Task<LoanDetailDto?> GetLoanDetailAsync(int loanId)
    {
        return await _mediator.Send(new ObtenerDetallePrestamoQuery { LoanId = loanId });
    }

    public async Task<EligibleClientsResultDto> GetEligibleClientsAsync(string? identificationNumber = null)
    {
        return await _mediator.Send(new GetEligibleClientsForLoanQuery { IdentificationNumber = identificationNumber });
    }

    public async Task<int> AssignLoanAsync(int userId, decimal disbursedAmount, decimal annualInterestRate, int termInMonths, bool confirmHighRisk)
    {
        return await _mediator.Send(new AsignarPrestamoCommand
        {
            UserId = userId,
            DisbursedAmount = disbursedAmount,
            AnnualInterestRate = annualInterestRate,
            TermInMonths = termInMonths,
            ConfirmHighRisk = confirmHighRisk
        });
    }

    public async Task<bool> EditLoanRateAsync(int loanId, decimal newAnnualInterestRate)
    {
        return await _mediator.Send(new EditarTasaPrestamoCommand
        {
            LoanId = loanId,
            NewAnnualInterestRate = newAnnualInterestRate
        });
    }
}
