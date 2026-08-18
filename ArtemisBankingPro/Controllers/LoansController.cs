using ArtemisBankingPro.Core.Application.Features.Loans.Queries.GetLoans;
using ArtemisBankingPro.Core.Application.Features.Loans.Queries.ObtenerDetallePrestamo;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.WebApp.Controllers;

[AllowAnonymous]
public class LoansController : Controller
{
    private readonly IMediator _mediator;

    public LoansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(
        int pageNumber = 1, int pageSize = 10, string? identificationNumber = null, LoanStatus? status = null)
    {
        var query = new GetLoansQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IdentificationNumber = identificationNumber,
            Status = status
        };

        var result = await _mediator.Send(query);
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var query = new ObtenerDetallePrestamoQuery { LoanId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return View(result);
    }
}
