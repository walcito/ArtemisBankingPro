using MediatR;
using ArtemisBankingPro.Core.Application.Dtos.Client;
using ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetBeneficiaries;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetBeneficiaries;

public class GetBeneficiariesQuery : IRequest<List<BeneficiaryDto>>
{
    public int OwnerUserId { get; set; }
}