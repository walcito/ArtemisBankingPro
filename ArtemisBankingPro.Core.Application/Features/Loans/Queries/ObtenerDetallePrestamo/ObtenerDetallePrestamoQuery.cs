using MediatR;
using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.ObtenerDetallePrestamo;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Queries.ObtenerDetallePrestamo;

public class ObtenerDetallePrestamoQuery : IRequest<DetallePrestamoDto?>
{
    public int LoanId { get; set; }
}
