using ArtemisBankingPro.Core.Application.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Users;

public class ChangeUserStatusCommand : IRequest<bool>
{
    public string Id { get; set; } = string.Empty;
    public bool Status { get; set; }
}

public class ChangeUserStatusCommandHandler : IRequestHandler<ChangeUserStatusCommand, bool>
{
    private readonly IUserService _userService;

    public ChangeUserStatusCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
    {
        await _userService.ChangeStatusAsync(request.Id, request.Status);
        return true;
    }
}