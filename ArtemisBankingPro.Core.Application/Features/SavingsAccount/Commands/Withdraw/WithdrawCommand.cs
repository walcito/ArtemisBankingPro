using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Withdraw;

public class WithdrawCommand : IRequest<Unit>
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}