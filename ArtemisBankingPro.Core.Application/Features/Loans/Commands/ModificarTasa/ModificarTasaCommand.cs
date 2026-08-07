using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Commands.ModificarTasa;

public class ModificarTasaCommand : IRequest<bool>
{
    public int LoanId { get; set; }
    public decimal NewAnnualInterestRate { get; set; }
}
