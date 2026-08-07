using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.AsignarPrestamo;

public class AsignarPrestamoCommand : IRequest<int>
{
    public int UserId { get; set; }
    public decimal DisbursedAmount { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int TermInMonths { get; set; }
}
