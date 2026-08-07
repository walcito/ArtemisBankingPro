using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.DesembolsarPrestamo;

public class DesembolsarPrestamoCommand : IRequest<bool>
{
    public int LoanId { get; set; }
}
