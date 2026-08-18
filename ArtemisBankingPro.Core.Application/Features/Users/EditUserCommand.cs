using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.Users;

public class EditUserCommand : IRequest<bool>
{
    public string Id { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
}

public class EditUserCommandHandler : IRequestHandler<EditUserCommand, bool>
{
    private readonly IUserService _userService;

    public EditUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> Handle(EditUserCommand request, CancellationToken cancellationToken)
    {
        await _userService.UpdateAsync(request.User, request.Id);
        return true;
    }
}