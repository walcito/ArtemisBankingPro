using ArtemisBankingPro.Core.Application.Dtos.Loan;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetLoans;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ArtemisBankingPro.WebAPI.Controllers;

[ApiController]
[Route("api/loans")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAmortizationService _amortizationService;

    public LoansController(IMediator mediator, IAmortizationService amortizationService)
    {
        _mediator = mediator;
        _amortizationService = amortizationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetLoansQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // Simula una tabla de amortización sin persistir datos
    [HttpPost("simulate")]
    [AllowAnonymous]
    public IActionResult Simulate([FromBody] SimulateLoanRequest request)
    {
        if (request.DisbursedAmount <= 0 || request.AnnualInterestRate < 0 || request.TermInMonths <= 0)
            return BadRequest("Los valores del préstamo deben ser positivos.");

        var table = _amortizationService.GenerateAmortizationTable(
            request.DisbursedAmount,
            request.AnnualInterestRate,
            request.TermInMonths
        );

        var response = table.Select(i => new
        {
            i.InstallmentNumber,
            i.InstallmentAmount,
            i.Interest,
            i.PrincipalAmortization,
            i.PendingBalance
        });

        return Ok(response);
    }
}

