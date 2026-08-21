using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.Loans;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;
using ArtemisBankingPro.Core.Application.Dtos.Client;
using ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetClientHome;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetClientHome;

public class GetClientHomeQueryHandler : IRequestHandler<GetClientHomeQuery, ClientHomeDto>
{
    private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
    private readonly IGenericRepository<Loan> _loanRepository;
    private readonly IGenericRepository<CreditCard> _creditCardRepository;

    public GetClientHomeQueryHandler(
        IGenericRepository<SavingsAccount> savingsAccountRepository,
        IGenericRepository<Loan> loanRepository,
        IGenericRepository<CreditCard> creditCardRepository)
    {
        _savingsAccountRepository = savingsAccountRepository;
        _loanRepository = loanRepository;
        _creditCardRepository = creditCardRepository;
    }

    public async Task<ClientHomeDto> Handle(GetClientHomeQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _savingsAccountRepository.GetAllAsync();
        var loans = await _loanRepository.GetAllAsync();
        var creditCards = await _creditCardRepository.GetAllAsync();

        var dto = new ClientHomeDto
        {
            SavingsAccounts = accounts
                .Where(a => a.UserId == request.UserId)
                .Select(a => new ProductSummaryDto
                {
                    Id = a.Id,
                    Identifier = a.AccountNumber,
                    RelevantAmount = a.Balance,
                    Status = a.Status.ToString()
                })
                .ToList(),

            Loans = loans
                .Where(l => l.UserId == request.UserId)
                .Select(l => new ProductSummaryDto
                {
                    Id = l.Id,
                    Identifier = l.LoanNumber,
                    RelevantAmount = l.CurrentDebt,
                    Status = l.Status.ToString()
                })
                .ToList(),

            CreditCards = creditCards
                .Where(c => c.UserId == request.UserId)
                .Select(c => new ProductSummaryDto
                {
                    Id = c.Id,
                    Identifier = MaskCardNumber(c.CardNumber),
                    RelevantAmount = c.CurrentDebt,
                    Status = c.Status.ToString()
                })
                .ToList()
        };

        return dto;
    }

    private static string MaskCardNumber(string cardNumber)
    {
        if (cardNumber.Length < 4)
            return cardNumber;

        return $"**** **** **** {cardNumber[^4..]}";
    }
}