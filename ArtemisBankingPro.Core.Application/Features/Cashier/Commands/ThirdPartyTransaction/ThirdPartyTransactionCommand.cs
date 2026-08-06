using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Commands.ThirdPartyTransaction;

public class ThirdPartyTransactionCommand : IRequest<Unit>
{
    public string SourceAccountNumber { get; set; } = string.Empty;
    public string DestinationAccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int CashierUserId { get; set; }
}