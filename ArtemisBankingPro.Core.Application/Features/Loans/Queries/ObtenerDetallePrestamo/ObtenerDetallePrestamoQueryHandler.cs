using ArtemisBankingPro.Core.Domain.Entities;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Queries.ObtenerDetallePrestamo;

public class ObtenerDetallePrestamoQueryHandler : IRequestHandler<ObtenerDetallePrestamoQuery, DetallePrestamoDto?>
{
    private readonly IGenericRepository<Loan> _loanRepository;

    public ObtenerDetallePrestamoQueryHandler(IGenericRepository<Loan> loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<DetallePrestamoDto?> Handle(ObtenerDetallePrestamoQuery request, CancellationToken cancellationToken)
    {
        // Obtener el préstamo con sus cuotas y el usuario dueño del préstamo
        var loans = await _loanRepository.GetAllListWithIncludeAsync(l => l.Installments, l => l.User!);
        var loan = loans.FirstOrDefault(l => l.Id == request.LoanId);

        if (loan is null)
            return null;

        var dto = new DetallePrestamoDto
        {
            Id = loan.Id,
            LoanNumber = loan.LoanNumber,
            DisbursedAmount = loan.DisbursedAmount,
            AnnualInterestRate = loan.AnnualInterestRate,
            TermInMonths = loan.TermInMonths,
            CurrentDebt = loan.CurrentDebt,
            Status = loan.Status,
            UserId = loan.UserId,
            UserFullName = loan.User != null ? $"{loan.User.Name} {loan.User.LastName}" : string.Empty,
            UserIdentificationNumber = loan.User?.IdentificationNumber ?? string.Empty,
            Installments = loan.Installments
                .OrderBy(i => i.InstallmentNumber)
                .Select(i => new CuotaPrestamoDto
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
