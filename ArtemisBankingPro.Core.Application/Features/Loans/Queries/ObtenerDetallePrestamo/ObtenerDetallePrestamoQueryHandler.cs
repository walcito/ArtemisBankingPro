using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;
using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.ObtenerDetallePrestamo;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Queries.ObtenerDetallePrestamo;

public class ObtenerDetallePrestamoQueryHandler : IRequestHandler<ObtenerDetallePrestamoQuery, LoanDetailDto?>
{
    private readonly IGenericRepository<Domain.Entities.Loans.Loan> _loanRepository;

    public ObtenerDetallePrestamoQueryHandler(IGenericRepository<Domain.Entities.Loans.Loan> loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<LoanDetailDto?> Handle(ObtenerDetallePrestamoQuery request, CancellationToken cancellationToken)
    {
        var loans = await _loanRepository.GetAllListWithIncludeAsync(l => l.Installments, l => l.User!);
        var loan = loans.FirstOrDefault(l => l.Id == request.LoanId);

        if (loan == null) return null;

        var dto = new LoanDetailDto
        {
            Id = loan.Id,
            LoanNumber = loan.LoanNumber,
            DisbursedAmount = loan.DisbursedAmount,
            AnnualInterestRate = loan.AnnualInterestRate,
            TermInMonths = loan.TermInMonths,
            CurrentDebt = loan.CurrentDebt,
            Status = loan.Status,
            UserId = loan.UserId,
            UserFullName = loan.User != null ? $"{loan.User.FirstName} {loan.User.LastName}" : string.Empty,
            UserIdentificationNumber = loan.User != null ? loan.User.IdentificationNumber : string.Empty,
            Installments = loan.Installments
                .OrderBy(i => i.InstallmentNumber)
                .Select(i => new LoanInstallmentDto
                {
                    Id = i.Id,
                    InstallmentNumber = i.InstallmentNumber,
                    InstallmentAmount = i.InstallmentAmount,
                    Interest = i.Interest,
                    PrincipalAmortization = i.PrincipalAmortization,
                    PendingBalance = i.PendingBalance,
                    Status = i.Status,
                    PaidAmount = i.PaidAmount
                }).ToList()
        };

        return dto;
    }
}
