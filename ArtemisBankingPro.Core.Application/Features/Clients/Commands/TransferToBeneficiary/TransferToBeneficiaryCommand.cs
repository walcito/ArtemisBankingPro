using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferToBeneficiary;

public class TransferToBeneficiaryCommand : IRequest<Unit>
{
    public string SourceAccountNumber { get; set; } = string.Empty;
    public int BeneficiaryId { get; set; }
    public decimal Amount { get; set; }
    public int RequestingUserId { get; set; }
}