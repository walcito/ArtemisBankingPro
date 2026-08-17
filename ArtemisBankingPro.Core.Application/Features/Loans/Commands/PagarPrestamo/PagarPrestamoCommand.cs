using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.PagarPrestamo;

public class PagarPrestamoCommand : IRequest<bool>
{
    public int LoanId { get; set; }
    public decimal Amount { get; set; }
    public int SavingsAccountId { get; set; }
}
