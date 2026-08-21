using ArtemisBankingPro.Core.Application.Dtos.User;
using ArtemisBankingPro.Core.Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingProApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin,Administrador")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _mediator.Send(new GetUsersQuery());
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var users = await _mediator.Send(new GetUsersQuery());
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound("Usuario no encontrado.");
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var result = await _mediator.Send(new CreateUserCommand { User = dto });
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UserDto userDto)
    {
        userDto.Id = id;
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
