using ArtemisBankingPro.Core.Application.Common.Models;
using ArtemisBankingPro.Core.Application.Dtos.SavingsAccount;
using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;

namespace ArtemisBankingPro.Core.Application.Interfaces;

public interface ISavingsAccountService
{
    Task<PagedResult<SavingsAccountDto>> GetSavingsAccountsAsync(int pageNumber, int pageSize, string? identificationNumber, AccountStatus? status, AccountType? type);
    Task<PagedResult<TransactionDto>> GetAccountTransactionsAsync(string accountNumber, int pageNumber, int pageSize);
    Task CreateSavingsAccountAsync(int userId, decimal initialAmount);
    Task CancelSavingsAccountAsync(string accountNumber);
    Task<List<UserDto>> GetActiveClientsAsync(string? identificationNumber = null);
}
