using MediatR;
using ArtemisBankingPro.Core.Application.Dtos.Cashier;
using ArtemisBankingPro.Core.Application.Features.Cashier.Queries.GetCashierHome;

namespace ArtemisBankingPro.Core.Application.Features.Cashier.Queries.GetCashierHome;

public class GetCashierHomeQuery : IRequest<CashierHomeDto>
{
    public int CashierUserId { get; set; }
}