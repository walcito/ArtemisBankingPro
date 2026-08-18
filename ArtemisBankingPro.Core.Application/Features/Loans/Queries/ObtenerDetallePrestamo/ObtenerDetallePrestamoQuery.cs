using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Queries.ObtenerDetallePrestamo;

public class ObtenerDetallePrestamoQuery : IRequest<DetallePrestamoDto?>
{
    public int LoanId { get; set; }
}
