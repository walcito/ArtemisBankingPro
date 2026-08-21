using ArtemisBankingPro.Core.Application.Dtos.Client;

namespace ArtemisBankingPro.Core.Application.Interfaces.Client;

public interface IClientProductService
{
    Task<ClientHomeDto> GetClientHomeAsync(int userId);
    Task<List<BeneficiaryDto>> GetBeneficiariesAsync(int ownerUserId);
    Task<int> AddBeneficiaryAsync(int ownerUserId, string beneficiaryAccountNumber, string alias);
    Task RemoveBeneficiaryAsync(int id, int ownerUserId);
    Task TransferBetweenOwnAccountsAsync(string sourceAccountNumber, string destinationAccountNumber, decimal amount, int requestingUserId);
    Task ExpressTransactionAsync(string sourceAccountNumber, string destinationAccountNumber, decimal amount, int requestingUserId);
    Task TransferToBeneficiaryAsync(string sourceAccountNumber, int beneficiaryId, decimal amount, int requestingUserId);
    Task CashAdvanceAsync(int creditCardId, string destinationAccountNumber, decimal amount, int requestingUserId);
    Task PayLoanAsync(string sourceAccountNumber, int loanId, decimal amount, int requestingUserId);
    Task PayCreditCardAsync(string sourceAccountNumber, int creditCardId, decimal amount, int requestingUserId);
}
