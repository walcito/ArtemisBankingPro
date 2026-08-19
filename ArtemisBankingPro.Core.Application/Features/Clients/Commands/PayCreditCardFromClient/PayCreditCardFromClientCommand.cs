using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayCreditCardFromClient;

public class PayCreditCardFromClientCommand : IRequest<Unit>
{
    public string SourceAccountNumber { get; set; } = string.Empty;
    public int CreditCardId { get; set; }
    public decimal Amount { get; set; }
    public int RequestingUserId { get; set; }
}