using ArtemisBankingPro.Core.Application.Features.Loans.Commands.AsignarPrestamo;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetLoans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.WebAPI.Controllers;

[ApiController]
[Route("api/loans")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetLoansQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AsignarPrestamo([FromBody] AsignarPrestamoCommand command)
    {
        var loanId = await _mediator.Send(command);
        return Ok(loanId);
    }
}
