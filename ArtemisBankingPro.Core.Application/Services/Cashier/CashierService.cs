using ArtemisBankingPro.Core.Application.Dtos.Cashier;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayCreditCard;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayLoan;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.ThirdPartyTransaction;
using ArtemisBankingPro.Core.Application.Features.Cashier.Queries.GetCashierHome;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Deposit;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Withdraw;
using ArtemisBankingPro.Core.Application.Interfaces.Cashier;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Services.Cashier;

public class CashierService : ICashierService
{
    private readonly IMediator _mediator;

    public CashierService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<CashierHomeDto> GetCashierHomeAsync(int cashierUserId)
    {
        return await _mediator.Send(new GetCashierHomeQuery { CashierUserId = cashierUserId });
    }

    public async Task DepositAsync(string accountNumber, decimal amount)
    {
        await _mediator.Send(new DepositCommand { AccountNumber = accountNumber, Amount = amount });
    }

    public async Task WithdrawAsync(string accountNumber, decimal amount)
    {
        await _mediator.Send(new WithdrawCommand { AccountNumber = accountNumber, Amount = amount });
    }

    public async Task ThirdPartyTransactionAsync(string sourceAccountNumber, string destinationAccountNumber, decimal amount, int cashierUserId)
    {
        await _mediator.Send(new ThirdPartyTransactionCommand
        {
            SourceAccountNumber = sourceAccountNumber,
            DestinationAccountNumber = destinationAccountNumber,
            Amount = amount,
            CashierUserId = cashierUserId
        });
    }

    public async Task PayLoanAsync(string sourceAccountNumber, int loanId, decimal amount, int cashierUserId)
    {
        await _mediator.Send(new PayLoanCommand
        {
            SourceAccountNumber = sourceAccountNumber,
            LoanId = loanId,
            Amount = amount,
            CashierUserId = cashierUserId
        });
    }

    public async Task PayCreditCardAsync(string sourceAccountNumber, int creditCardId, decimal amount, int cashierUserId)
    {
        await _mediator.Send(new PayCreditCardCommand
        {
            SourceAccountNumber = sourceAccountNumber,
            CreditCardId = creditCardId,
            Amount = amount,
            CashierUserId = cashierUserId
        });
    }
}
