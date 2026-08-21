using ArtemisBankingPro.Core.Application.Dtos.Loan;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetEligibleClients;

public class GetEligibleClientsForLoanQuery : IRequest<EligibleClientsResultDto>
{
    public string? IdentificationNumber { get; set; }
}
