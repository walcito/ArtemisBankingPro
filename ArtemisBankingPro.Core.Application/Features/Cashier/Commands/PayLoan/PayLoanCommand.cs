using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayLoan;

public class PayLoanCommand : IRequest<Unit>
{
    public string SourceAccountNumber { get; set; } = string.Empty;
    public int LoanId { get; set; }
    public decimal Amount { get; set; }
    public int CashierUserId { get; set; }
}