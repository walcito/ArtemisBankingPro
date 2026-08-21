using ArtemisBankingPro.Core.Application.Features.Trades.Commands.CreateTrade;
using ArtemisBankingPro.Core.Application.Features.Trades.Commands.UpdateTrade;
using ArtemisBankingPro.Core.Application.Features.Trades.Commands.ToggleTradeStatus;
using ArtemisBankingPro.Core.Application.Dtos.Commerce;
using ArtemisBankingPro.Core.Application.Dtos.Commerce;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTradeById;
using ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTrades;

namespace ArtemisBankingPro.WebAPI.Controllers;

[ApiController]
[Route("api/commerce")]
[Authorize]
public class CommerceController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommerceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetTradesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetTradeByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTradeCommand command)
    {
        var newTradeId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = newTradeId }, newTradeId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTradeCommand command)
    {
        if (id != command.Id)
            return BadRequest("El Id de la ruta no coincide con el del cuerpo de la petición.");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        await _mediator.Send(new ToggleTradeStatusCommand { Id = id });
        return NoContent();
    }
}