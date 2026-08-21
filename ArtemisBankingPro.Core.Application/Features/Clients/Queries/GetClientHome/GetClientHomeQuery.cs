using MediatR;
using ArtemisBankingPro.Core.Application.Dtos.Client;
using ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetClientHome;

namespace ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetClientHome;

public class GetClientHomeQuery : IRequest<ClientHomeDto>
{
    public int UserId { get; set; }
}