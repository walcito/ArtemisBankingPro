using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayLoanFromClient;

public class PayLoanFromClientCommand : IRequest<Unit>
{
    public string SourceAccountNumber { get; set; } = string.Empty;
    public int LoanId { get; set; }
    public decimal Amount { get; set; }
    public int RequestingUserId { get; set; }
}