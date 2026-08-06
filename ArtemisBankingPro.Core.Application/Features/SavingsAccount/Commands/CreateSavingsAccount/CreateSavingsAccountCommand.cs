using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;

public class CreateSavingsAccountCommand : IRequest<int>
{
    public int UserId { get; set; }
    public decimal InitialAmount { get; set; } = 0.00m;
}