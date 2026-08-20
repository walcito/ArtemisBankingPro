using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingProApi.Controllers;

[ApiController]
[Route("api/commerce-users")]
[Authorize(Roles = "Administrador")]
public class CommerceUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommerceUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCommerceUsers()
    {
        var users = await _mediator.Send(new GetCommerceUsersQuery());
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCommerceUser([FromBody] CreateCommerceUserDto dto)
    {
        var createUserDto = new CreateUserDto
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Identification = dto.Identification,
            Email = dto.Email,
            UserName = dto.UserName,
            Password = dto.Password,
            ConfirmPassword = dto.ConfirmPassword,
            Role = "Commerce",
            InitialAmount = dto.InitialAmount
        };

        var result = await _mediator.Send(new CreateUserCommand { User = createUserDto });
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCommerceUser(string id, [FromBody] EditUserDto dto)
    {
        var userDto = new UserDto
        {
            Id = id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Identification = dto.Identification,
            Email = dto.Email,
            UserName = dto.UserName,
            Role = "Commerce"
        };

        var result = await _mediator.Send(new EditUserCommand { Id = id, User = userDto });
        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStatus(string id, [FromBody] bool status)
    {
        await _mediator.Send(new ChangeUserStatusCommand { Id = id, Status = status });
        return NoContent();
    }
}
