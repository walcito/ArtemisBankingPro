using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Deposit;

public class DepositCommand : IRequest<Unit>
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}