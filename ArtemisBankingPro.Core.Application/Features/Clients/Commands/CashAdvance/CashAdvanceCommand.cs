using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.CashAdvance;

public class CashAdvanceCommand : IRequest<Unit>
{
    public int CreditCardId { get; set; }
    public string DestinationAccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int RequestingUserId { get; set; }
}