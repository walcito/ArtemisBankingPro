using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;

public class CancelSavingsAccountCommand : IRequest<Unit>
{
    public string AccountNumber { get; set; } = string.Empty;
}