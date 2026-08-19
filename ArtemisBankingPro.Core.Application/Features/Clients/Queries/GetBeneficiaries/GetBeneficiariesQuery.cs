using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetBeneficiaries;

public class GetBeneficiariesQuery : IRequest<List<BeneficiaryDto>>
{
    public int OwnerUserId { get; set; }
}