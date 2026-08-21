using System.Security.Claims;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.ThirdPartyTransaction;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayLoan;
using ArtemisBankingPro.Core.Application.Dtos.Cashier;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Deposit;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Withdraw;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayCreditCard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtemisBankingPro.Core.Application.Features.Cashier.Queries.GetCashierHome;

namespace ArtemisBankingPro.WebApp.Controllers;

[Authorize(Roles = "Cashier,Cajero,Teller")]
public class CashierController : Controller
{
    private readonly IMediator _mediator;

    public CashierController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetAuthenticatedCashierId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (idClaim is null || !int.TryParse(idClaim, out var cashierId))
            throw new Exception("No se pudo identificar al cajero autenticado.");

        return cashierId;
    }

    public async Task<IActionResult> Index()
    {
        var query = new GetCashierHomeQuery { CashierUserId = GetAuthenticatedCashierId() };
        var result = await _mediator.Send(query);
        return View(result);
    }

    public IActionResult Deposit() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deposit(DepositCommand command)
    {
        if (!ModelState.IsValid)
            return View(command);

        await _mediator.Send(command);
        TempData["SuccessMessage"] = "Depósito realizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Withdraw() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(WithdrawCommand command)
    {
        if (!ModelState.IsValid)
            return View(command);

        await _mediator.Send(command);
        TempData["SuccessMessage"] = "Retiro realizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult ThirdPartyTransaction() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThirdPartyTransaction(ThirdPartyTransactionCommand command)
    {
        command.CashierUserId = GetAuthenticatedCashierId();

        if (!ModelState.IsValid)
            return View(command);

        await _mediator.Send(command);
        TempData["SuccessMessage"] = "Transacción realizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult PayLoan() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayLoan(PayLoanCommand command)
    {
        command.CashierUserId = GetAuthenticatedCashierId();

        if (!ModelState.IsValid)
            return View(command);

        await _mediator.Send(command);
        TempData["SuccessMessage"] = "Pago de préstamo procesado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("pay-credit-card")]
    public async Task<IActionResult> PayCreditCard([FromBody] PayCreditCardCommand command)
    {
        command.CashierUserId = GetAuthenticatedCashierId();
        await _mediator.Send(command);
        return NoContent();
    }
}