using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.ExpressTransaction;

public class ExpressTransactionCommand : IRequest<Unit>
{
    public string SourceAccountNumber { get; set; } = string.Empty;
    public string DestinationAccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int RequestingUserId { get; set; }
}