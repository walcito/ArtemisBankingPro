using System.Security.Claims;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.ThirdPartyTransaction;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayLoan;
using ArtemisBankingPro.Core.Application.Features.Cashier.Queries.GetCashierHome;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Deposit;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Withdraw;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayCreditCard;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.WebApp.Controllers;

[Authorize(Roles = "Cajero")]
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
    public IActionResult Deposit(DepositCommand command)
    {
        if (!ModelState.IsValid)
            return View(command);

        // Guardamos el Command temporalmente, para reutilizarlo en la pantalla de confirmación
        TempData["PendingDeposit"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmDeposit));
    }

    public IActionResult ConfirmDeposit()
    {
        if (TempData.Peek("PendingDeposit") is not string json)
            return RedirectToAction(nameof(Deposit));

        var command = JsonSerializer.Deserialize<DepositCommand>(json);
        return View(command);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDeposit(bool confirmed)
    {
        if (TempData["PendingDeposit"] is not string json)
            return RedirectToAction(nameof(Deposit));

        var command = JsonSerializer.Deserialize<DepositCommand>(json);

        if (!confirmed || command is null)
            return RedirectToAction(nameof(Deposit));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se acreditaron {command.Amount:C2} a la cuenta {command.AccountNumber}.";
        return RedirectToAction(nameof(DepositSuccess));
    }

    public IActionResult DepositSuccess() => View();

    public IActionResult Withdraw() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Withdraw(WithdrawCommand command)
    {
        if (!ModelState.IsValid)
            return View(command);

        TempData["PendingWithdraw"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmWithdraw));
    }

    public IActionResult ConfirmWithdraw()
    {
        if (TempData.Peek("PendingWithdraw") is not string json)
            return RedirectToAction(nameof(Withdraw));

        var command = JsonSerializer.Deserialize<WithdrawCommand>(json);
        return View(command);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmWithdraw(bool confirmed)
    {
        if (TempData["PendingWithdraw"] is not string json)
            return RedirectToAction(nameof(Withdraw));

        var command = JsonSerializer.Deserialize<WithdrawCommand>(json);

        if (!confirmed || command is null)
            return RedirectToAction(nameof(Withdraw));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se descontaron {command.Amount:C2} de la cuenta {command.AccountNumber}.";
        return RedirectToAction(nameof(WithdrawSuccess));
    }

    public IActionResult WithdrawSuccess() => View();

    public IActionResult ThirdPartyTransaction() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ThirdPartyTransaction(ThirdPartyTransactionCommand command)
    {
        command.CashierUserId = GetAuthenticatedCashierId();

        if (!ModelState.IsValid)
            return View(command);

        TempData["PendingThirdParty"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmThirdPartyTransaction));
    }

    public IActionResult ConfirmThirdPartyTransaction()
    {
        if (TempData.Peek("PendingThirdParty") is not string json)
            return RedirectToAction(nameof(ThirdPartyTransaction));

        var command = JsonSerializer.Deserialize<ThirdPartyTransactionCommand>(json);
        return View(command);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmThirdPartyTransaction(bool confirmed)
    {
        if (TempData["PendingThirdParty"] is not string json)
            return RedirectToAction(nameof(ThirdPartyTransaction));

        var command = JsonSerializer.Deserialize<ThirdPartyTransactionCommand>(json);

        if (!confirmed || command is null)
            return RedirectToAction(nameof(ThirdPartyTransaction));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se transfirieron {command.Amount:C2} desde la cuenta {command.SourceAccountNumber} hacia la cuenta {command.DestinationAccountNumber}.";
        return RedirectToAction(nameof(ThirdPartyTransactionSuccess));
    }

    public IActionResult ThirdPartyTransactionSuccess() => View();
    public IActionResult PayLoan() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PayLoan(PayLoanCommand command)
    {
        command.CashierUserId = GetAuthenticatedCashierId();

        if (!ModelState.IsValid)
            return View(command);

        TempData["PendingPayLoan"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmPayLoan));
    }

    public IActionResult ConfirmPayLoan()
    {
        if (TempData.Peek("PendingPayLoan") is not string json)
            return RedirectToAction(nameof(PayLoan));

        var command = JsonSerializer.Deserialize<PayLoanCommand>(json);
        return View(command);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPayLoan(bool confirmed)
    {
        if (TempData["PendingPayLoan"] is not string json)
            return RedirectToAction(nameof(PayLoan));

        var command = JsonSerializer.Deserialize<PayLoanCommand>(json);

        if (!confirmed || command is null)
            return RedirectToAction(nameof(PayLoan));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se aplicó un abono de {command.Amount:C2} al préstamo #{command.LoanId}.";
        return RedirectToAction(nameof(PayLoanSuccess));
    }

    public IActionResult PayLoanSuccess() => View();

    public IActionResult PayCreditCard() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PayCreditCard(PayCreditCardCommand command)
    {
        command.CashierUserId = GetAuthenticatedCashierId();

        if (!ModelState.IsValid)
            return View(command);

        TempData["PendingPayCreditCard"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmPayCreditCard));
    }

    public IActionResult ConfirmPayCreditCard()
    {
        if (TempData.Peek("PendingPayCreditCard") is not string json)
            return RedirectToAction(nameof(PayCreditCard));

        var command = JsonSerializer.Deserialize<PayCreditCardCommand>(json);
        return View(command);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPayCreditCard(bool confirmed)
    {
        if (TempData["PendingPayCreditCard"] is not string json)
            return RedirectToAction(nameof(PayCreditCard));

        var command = JsonSerializer.Deserialize<PayCreditCardCommand>(json);

        if (!confirmed || command is null)
            return RedirectToAction(nameof(PayCreditCard));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se aplicó un pago de {command.Amount:C2} a la tarjeta #{command.CreditCardId}.";
        return RedirectToAction(nameof(PayCreditCardSuccess));
    }

    public IActionResult PayCreditCardSuccess() => View();
}