using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Features.Users;
using ArtemisBankingPro.Core.Application.Interfaces;
using MediatR;

namespace ArtemisBankingPro.Core.Application.Services;

public class AdminService : IAdminService
{
    private readonly IMediator _mediator;

    public AdminService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        return await _mediator.Send(new GetUsersQuery());
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var users = await _mediator.Send(new GetUsersQuery());
        return users.FirstOrDefault(u => u.Id == id);
    }

    public async Task CreateUserAsync(CreateUserDto dto)
    {
        await _mediator.Send(new CreateUserCommand { User = dto });
    }

    public async Task EditUserAsync(string id, UserDto dto)
    {
        await _mediator.Send(new EditUserCommand { Id = id, User = dto });
    }

    public async Task ChangeUserStatusAsync(string id, bool status)
    {
        await _mediator.Send(new ChangeUserStatusCommand { Id = id, Status = status });
    }
}
