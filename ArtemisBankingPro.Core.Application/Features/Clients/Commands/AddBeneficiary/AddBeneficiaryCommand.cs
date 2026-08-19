using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.AddBeneficiary;

public class AddBeneficiaryCommand : IRequest<int>
{
    public int OwnerUserId { get; set; }
    public string BeneficiaryAccountNumber { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
}