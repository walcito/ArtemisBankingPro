using ArtemisBankingPro.Core.Application.Dtos.Client;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.AddBeneficiary;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.CashAdvance;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.ExpressTransaction;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayCreditCardFromClient;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayLoanFromClient;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.RemoveBeneficiary;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferBetweenOwnAccounts;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferToBeneficiary;
using ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetBeneficiaries;
using ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetClientHome;
using ArtemisBankingPro.Core.Application.Interfaces.Client;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Services.Client;

public class ClientProductService : IClientProductService
{
    private readonly IMediator _mediator;

    public ClientProductService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ClientHomeDto> GetClientHomeAsync(int userId)
    {
        return await _mediator.Send(new GetClientHomeQuery { UserId = userId });
    }

    public async Task<List<BeneficiaryDto>> GetBeneficiariesAsync(int ownerUserId)
    {
        return await _mediator.Send(new GetBeneficiariesQuery { OwnerUserId = ownerUserId });
    }

    public async Task<int> AddBeneficiaryAsync(int ownerUserId, string beneficiaryAccountNumber, string alias)
    {
        return await _mediator.Send(new AddBeneficiaryCommand
        {
            OwnerUserId = ownerUserId,
            BeneficiaryAccountNumber = beneficiaryAccountNumber,
            Alias = alias
        });
    }

    public async Task RemoveBeneficiaryAsync(int id, int ownerUserId)
    {
        await _mediator.Send(new RemoveBeneficiaryCommand { Id = id, OwnerUserId = ownerUserId });
    }

    public async Task TransferBetweenOwnAccountsAsync(string sourceAccountNumber, string destinationAccountNumber, decimal amount, int requestingUserId)
    {
        await _mediator.Send(new TransferBetweenOwnAccountsCommand
        {
            SourceAccountNumber = sourceAccountNumber,
            DestinationAccountNumber = destinationAccountNumber,
            Amount = amount,
            RequestingUserId = requestingUserId
        });
    }

    public async Task ExpressTransactionAsync(string sourceAccountNumber, string destinationAccountNumber, decimal amount, int requestingUserId)
    {
        await _mediator.Send(new ExpressTransactionCommand
        {
            SourceAccountNumber = sourceAccountNumber,
            DestinationAccountNumber = destinationAccountNumber,
            Amount = amount,
            RequestingUserId = requestingUserId
        });
    }

    public async Task TransferToBeneficiaryAsync(string sourceAccountNumber, int beneficiaryId, decimal amount, int requestingUserId)
    {
        await _mediator.Send(new TransferToBeneficiaryCommand
        {
            SourceAccountNumber = sourceAccountNumber,
            BeneficiaryId = beneficiaryId,
            Amount = amount,
            RequestingUserId = requestingUserId
        });
    }

    public async Task CashAdvanceAsync(int creditCardId, string destinationAccountNumber, decimal amount, int requestingUserId)
    {
        await _mediator.Send(new CashAdvanceCommand
        {
            CreditCardId = creditCardId,
            DestinationAccountNumber = destinationAccountNumber,
            Amount = amount,
            RequestingUserId = requestingUserId
        });
    }

    public async Task PayLoanAsync(string sourceAccountNumber, int loanId, decimal amount, int requestingUserId)
    {
        await _mediator.Send(new PayLoanFromClientCommand
        {
            SourceAccountNumber = sourceAccountNumber,
            LoanId = loanId,
            Amount = amount,
            RequestingUserId = requestingUserId
        });
    }

    public async Task PayCreditCardAsync(string sourceAccountNumber, int creditCardId, decimal amount, int requestingUserId)
    {
        await _mediator.Send(new PayCreditCardFromClientCommand
        {
            SourceAccountNumber = sourceAccountNumber,
            CreditCardId = creditCardId,
            Amount = amount,
            RequestingUserId = requestingUserId
        });
    }
}
