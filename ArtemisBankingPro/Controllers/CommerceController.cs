using ArtemisBankingPro.Core.Application.Features.Trades.Commands.CreateTrade;
using ArtemisBankingPro.Core.Application.Features.Trades.Commands.UpdateTrade;
using ArtemisBankingPro.Core.Application.Features.Trades.Commands.ToggleTradeStatus;
using ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTrades;
using ArtemisBankingPro.Core.Application.Features.Trades.Queries.GetTradeById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class CommerceController : Controller
{
    private readonly IMediator _mediator;

    public CommerceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, bool? isActive = null)
    {
        var query = new GetTradesQuery { PageNumber = pageNumber, IsActive = isActive };
        var result = await _mediator.Send(query);
        return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _mediator.Send(new GetTradeByIdQuery { Id = id });
        return View(result);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTradeCommand command)
    {
        if (!ModelState.IsValid)
            return View(command);

        await _mediator.Send(command);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var trade = await _mediator.Send(new GetTradeByIdQuery { Id = id });

        var command = new UpdateTradeCommand
        {
            Id = trade.Id,
            Name = trade.Name,
            Description = trade.Description,
            Mail = trade.Mail,
            Phone = trade.Phone
        };

        return View(command);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateTradeCommand command)
    {
        if (!ModelState.IsValid)
            return View(command);

        await _mediator.Send(command);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        await _mediator.Send(new ToggleTradeStatusCommand { Id = id });
        return RedirectToAction(nameof(Index));
    }
}