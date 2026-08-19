using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferBetweenOwnAccounts;

public class TransferBetweenOwnAccountsCommand : IRequest<Unit>
{
    public string SourceAccountNumber { get; set; } = string.Empty;
    public string DestinationAccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int RequestingUserId { get; set; }
}