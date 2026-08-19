using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetClientHome;

public class GetClientHomeQuery : IRequest<ClientHomeDto>
{
    public int UserId { get; set; }
}