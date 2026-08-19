using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.RemoveBeneficiary;

public class RemoveBeneficiaryCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public int OwnerUserId { get; set; }
}