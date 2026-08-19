using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.EditarTasaPrestamo;

public class EditarTasaPrestamoCommand : IRequest<bool>
{
    public int LoanId { get; set; }
    public decimal NewAnnualInterestRate { get; set; }
}
