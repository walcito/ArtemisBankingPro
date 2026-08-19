using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Queries.GetCashierHome;

public class GetCashierHomeQuery : IRequest<CashierHomeDto>
{
    public int CashierUserId { get; set; }
}