using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayCreditCard;

public class PayCreditCardCommand : IRequest<Unit>
{
    public string SourceAccountNumber { get; set; } = string.Empty;
    public int CreditCardId { get; set; }
    public decimal Amount { get; set; }
    public int CashierUserId { get; set; }
} 