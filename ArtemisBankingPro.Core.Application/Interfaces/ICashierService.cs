using ArtemisBankingPro.Core.Application.Dtos.Cashier;

namespace ArtemisBankingPro.Core.Application.Interfaces;

public interface ICashierService
{
    Task<CashierHomeDto> GetCashierHomeAsync(int cashierUserId);
    Task DepositAsync(string accountNumber, decimal amount);
    Task WithdrawAsync(string accountNumber, decimal amount);
    Task ThirdPartyTransactionAsync(string sourceAccountNumber, string destinationAccountNumber, decimal amount, int cashierUserId);
    Task PayLoanAsync(string sourceAccountNumber, int loanId, decimal amount, int cashierUserId);
    Task PayCreditCardAsync(string sourceAccountNumber, int creditCardId, decimal amount, int cashierUserId);
}
