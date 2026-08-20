using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.Users;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Users;

public class GetCommerceUsersQuery : IRequest<List<UserDto>>
{
}

public class GetCommerceUsersQueryHandler : IRequestHandler<GetCommerceUsersQuery, List<UserDto>>
{
    private readonly IAccountService _accountService;

    public GetCommerceUsersQueryHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<List<UserDto>> Handle(GetCommerceUsersQuery request, CancellationToken cancellationToken)
    {
        return await _accountService.GetUsersByRoleAsync(Roles.Commerce.ToString());
    }
}