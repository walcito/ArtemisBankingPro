using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using MediatR;
using ArtemisBankingPro.Core.Application.Dtos.Cashier;
using ArtemisBankingPro.Core.Application.Features.Cashier.Queries.GetCashierHome;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Queries.GetCashierHome;

public class GetCashierHomeQueryHandler : IRequestHandler<GetCashierHomeQuery, CashierHomeDto>
{
    private readonly IGenericRepository<Transaction> _transactionRepository;

    public GetCashierHomeQueryHandler(IGenericRepository<Transaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<CashierHomeDto> Handle(GetCashierHomeQuery request, CancellationToken cancellationToken)
    {
        var allTransactions = await _transactionRepository.GetAllAsync();

        var today = DateTime.UtcNow.Date;

        var todaysTransactions = allTransactions
            .Where(t => t.PerformedByCashierId == request.CashierUserId && t.Date.Date == today)
            .ToList();

        var approvedToday = todaysTransactions
            .Where(t => t.Status == TransactionStatus.Approved)
            .ToList();

        var rejectedToday = todaysTransactions
            .Where(t => t.Status == TransactionStatus.Rejected)
            .ToList();

        return new CashierHomeDto
        {
            TotalTransactionsToday = approvedToday.Count,
            TotalAmountProcessedToday = approvedToday.Sum(t => t.Amount),
            RejectedTransactionsToday = rejectedToday.Count
        };
    }
}