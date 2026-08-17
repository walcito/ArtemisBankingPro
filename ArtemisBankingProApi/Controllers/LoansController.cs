using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Application.Features.Loans.Commands.AsignarPrestamo;
using ArtemisBankingPro.Core.Application.Features.Loans.Commands.EditarTasaPrestamo;
using ArtemisBankingPro.Core.Application.Features.Loans.Commands.PagarPrestamo;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetLoans;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.ObtenerDetallePrestamo;
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
        try
        {
            var loanId = await _mediator.Send(command);
            return Ok(loanId);
        }
        catch (ArtemisBankingPro.Core.Application.Exceptions.ClienteAltoRiesgoException ex)
        {
            return Conflict(new { error = ex.Message, confirmHighRisk = true });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        var query = new ObtenerDetallePrestamoQuery { LoanId = id };
        var result = await _mediator.Send(query);
        if (result is null)
            return NotFound("El préstamo no existe.");

        return Ok(result);
    }

    [HttpPatch("{id}/rate")]
    public async Task<IActionResult> EditarTasaPrestamo(int id, [FromBody] EditarTasaPrestamoRequest request)
    {
        var command = new EditarTasaPrestamoCommand
        {
            LoanId = id,
            NewAnnualInterestRate = request.NewAnnualInterestRate
        };
        var result = await _mediator.Send(command);
        if (!result)
            return NotFound("No se encontraron cuotas pendientes para recalcular o el préstamo no existe.");

        return NoContent();
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> PagarPrestamo(int id, [FromBody] PagarPrestamoCommand command)
    {
        if (id != command.LoanId)
            return BadRequest("El ID del préstamo no coincide.");

        var result = await _mediator.Send(command);
        if (!result)
            return BadRequest("No se pudo procesar el pago del préstamo.");

        return NoContent();
    }
}
