using System.Security.Claims;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.AddBeneficiary;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.RemoveBeneficiary;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferBetweenOwnAccounts;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.ExpressTransaction;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferToBeneficiary;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.CashAdvance;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayLoanFromClient;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayCreditCardFromClient;
using ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetClientHome;
using ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetBeneficiaries;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBankingPro.WebApp.Controllers;

[Authorize(Roles = "Client,Cliente")]
public class ClientController : Controller
{
    private readonly IMediator _mediator;

    public ClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetAuthenticatedUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(idClaim) && int.TryParse(idClaim, out var userId))
            return userId;

        var userRepo = HttpContext.RequestServices.GetService<IGenericRepository<User>>();
        var username = User.Identity?.Name;
        if (userRepo != null && !string.IsNullOrEmpty(username))
        {
            var users = userRepo.GetAllAsync().GetAwaiter().GetResult();
            var user = users.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) || 
                u.Email.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user != null)
                return user.Id;
        }

        return 1;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
        return View(result);
    }

    public async Task<IActionResult> Beneficiaries()
    {
        var result = await _mediator.Send(new GetBeneficiariesQuery { OwnerUserId = GetAuthenticatedUserId() });
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBeneficiary(AddBeneficiaryCommand command)
    {
        command.OwnerUserId = GetAuthenticatedUserId();
        await _mediator.Send(command);
        return RedirectToAction(nameof(Beneficiaries));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveBeneficiary(int id)
    {
        await _mediator.Send(new RemoveBeneficiaryCommand { Id = id, OwnerUserId = GetAuthenticatedUserId() });
        return RedirectToAction(nameof(Beneficiaries));
    }

    public async Task<IActionResult> TransferOwnAccounts()
    {
        var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
        ViewBag.Accounts = homeData.SavingsAccounts;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TransferOwnAccounts(TransferBetweenOwnAccountsCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        if (!ModelState.IsValid)
        {
            var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
            ViewBag.Accounts = homeData.SavingsAccounts;
            return View(command);
        }

        TempData["PendingTransferOwn"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmTransferOwnAccounts));
    }

    public IActionResult ConfirmTransferOwnAccounts()
    {
        if (TempData.Peek("PendingTransferOwn") is not string json)
            return RedirectToAction(nameof(TransferOwnAccounts));

        return View(JsonSerializer.Deserialize<TransferBetweenOwnAccountsCommand>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmTransferOwnAccounts(bool confirmed)
    {
        if (TempData["PendingTransferOwn"] is not string json)
            return RedirectToAction(nameof(TransferOwnAccounts));

        var command = JsonSerializer.Deserialize<TransferBetweenOwnAccountsCommand>(json);
        if (!confirmed || command is null) return RedirectToAction(nameof(TransferOwnAccounts));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se transfirieron {command.Amount:C2} entre tus cuentas propias.";
        return RedirectToAction(nameof(TransferOwnAccountsSuccess));
    }

    public IActionResult TransferOwnAccountsSuccess() => View();

    public async Task<IActionResult> ExpressTransaction()
    {
        var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
        ViewBag.Accounts = homeData.SavingsAccounts;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExpressTransaction(ExpressTransactionCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        if (!ModelState.IsValid)
        {
            var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
            ViewBag.Accounts = homeData.SavingsAccounts;
            return View(command);
        }

        TempData["PendingExpress"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmExpressTransaction));
    }

    public IActionResult ConfirmExpressTransaction()
    {
        if (TempData.Peek("PendingExpress") is not string json)
            return RedirectToAction(nameof(ExpressTransaction));

        return View(JsonSerializer.Deserialize<ExpressTransactionCommand>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmExpressTransaction(bool confirmed)
    {
        if (TempData["PendingExpress"] is not string json)
            return RedirectToAction(nameof(ExpressTransaction));

        var command = JsonSerializer.Deserialize<ExpressTransactionCommand>(json);
        if (!confirmed || command is null) return RedirectToAction(nameof(ExpressTransaction));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se transfirieron {command.Amount:C2} a la cuenta {command.DestinationAccountNumber}.";
        return RedirectToAction(nameof(ExpressTransactionSuccess));
    }

    public IActionResult ExpressTransactionSuccess() => View();

    public async Task<IActionResult> TransferToBeneficiary()
    {
        var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
        ViewBag.Accounts = homeData.SavingsAccounts;
        var beneficiaries = await _mediator.Send(new GetBeneficiariesQuery { OwnerUserId = GetAuthenticatedUserId() });
        ViewBag.Beneficiaries = beneficiaries;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TransferToBeneficiary(TransferToBeneficiaryCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();

        if (!ModelState.IsValid)
        {
            var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
            ViewBag.Accounts = homeData.SavingsAccounts;
            var beneficiaries = await _mediator.Send(new GetBeneficiariesQuery { OwnerUserId = GetAuthenticatedUserId() });
            ViewBag.Beneficiaries = beneficiaries;
            return View(command);
        }

        TempData["PendingTransferBeneficiary"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmTransferToBeneficiary));
    }

    public IActionResult ConfirmTransferToBeneficiary()
    {
        if (TempData.Peek("PendingTransferBeneficiary") is not string json)
            return RedirectToAction(nameof(TransferToBeneficiary));

        return View(JsonSerializer.Deserialize<TransferToBeneficiaryCommand>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmTransferToBeneficiary(bool confirmed)
    {
        if (TempData["PendingTransferBeneficiary"] is not string json)
            return RedirectToAction(nameof(TransferToBeneficiary));

        var command = JsonSerializer.Deserialize<TransferToBeneficiaryCommand>(json);
        if (!confirmed || command is null) return RedirectToAction(nameof(TransferToBeneficiary));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se transfirieron {command.Amount:C2} a tu beneficiario.";
        return RedirectToAction(nameof(TransferToBeneficiarySuccess));
    }

    public IActionResult TransferToBeneficiarySuccess() => View();

    public async Task<IActionResult> CashAdvance()
    {
        var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
        ViewBag.Accounts = homeData.SavingsAccounts;
        ViewBag.Cards = homeData.CreditCards;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CashAdvance(CashAdvanceCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        if (!ModelState.IsValid)
        {
            var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
            ViewBag.Accounts = homeData.SavingsAccounts;
            ViewBag.Cards = homeData.CreditCards;
            return View(command);
        }

        TempData["PendingCashAdvance"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmCashAdvance));
    }

    public IActionResult ConfirmCashAdvance()
    {
        if (TempData.Peek("PendingCashAdvance") is not string json)
            return RedirectToAction(nameof(CashAdvance));

        return View(JsonSerializer.Deserialize<CashAdvanceCommand>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmCashAdvance(bool confirmed)
    {
        if (TempData["PendingCashAdvance"] is not string json)
            return RedirectToAction(nameof(CashAdvance));

        var command = JsonSerializer.Deserialize<CashAdvanceCommand>(json);
        if (!confirmed || command is null) return RedirectToAction(nameof(CashAdvance));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se depositaron {command.Amount:C2} en tu cuenta.";
        return RedirectToAction(nameof(CashAdvanceSuccess));
    }

    public IActionResult CashAdvanceSuccess() => View();

    public async Task<IActionResult> PayLoan()
    {
        var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
        ViewBag.Accounts = homeData.SavingsAccounts;
        ViewBag.Loans = homeData.Loans;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayLoan(PayLoanFromClientCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        if (!ModelState.IsValid)
        {
            var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
            ViewBag.Accounts = homeData.SavingsAccounts;
            ViewBag.Loans = homeData.Loans;
            return View(command);
        }

        TempData["PendingPayLoan"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmPayLoan));
    }

    public IActionResult ConfirmPayLoan()
    {
        if (TempData.Peek("PendingPayLoan") is not string json)
            return RedirectToAction(nameof(PayLoan));

        return View(JsonSerializer.Deserialize<PayLoanFromClientCommand>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPayLoan(bool confirmed)
    {
        if (TempData["PendingPayLoan"] is not string json)
            return RedirectToAction(nameof(PayLoan));

        var command = JsonSerializer.Deserialize<PayLoanFromClientCommand>(json);
        if (!confirmed || command is null) return RedirectToAction(nameof(PayLoan));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se aplicó un abono de {command.Amount:C2} a tu préstamo.";
        return RedirectToAction(nameof(PayLoanSuccess));
    }

    public IActionResult PayLoanSuccess() => View();

    public async Task<IActionResult> PayCreditCard()
    {
        var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
        ViewBag.Accounts = homeData.SavingsAccounts;
        ViewBag.Cards = homeData.CreditCards;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayCreditCard(PayCreditCardFromClientCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        if (!ModelState.IsValid)
        {
            var homeData = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
            ViewBag.Accounts = homeData.SavingsAccounts;
            ViewBag.Cards = homeData.CreditCards;
            return View(command);
        }

        TempData["PendingPayCreditCard"] = JsonSerializer.Serialize(command);
        return RedirectToAction(nameof(ConfirmPayCreditCard));
    }

    public IActionResult ConfirmPayCreditCard()
    {
        if (TempData.Peek("PendingPayCreditCard") is not string json)
            return RedirectToAction(nameof(PayCreditCard));

        return View(JsonSerializer.Deserialize<PayCreditCardFromClientCommand>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPayCreditCard(bool confirmed)
    {
        if (TempData["PendingPayCreditCard"] is not string json)
            return RedirectToAction(nameof(PayCreditCard));

        var command = JsonSerializer.Deserialize<PayCreditCardFromClientCommand>(json);
        if (!confirmed || command is null) return RedirectToAction(nameof(PayCreditCard));

        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se aplicó un pago de {command.Amount:C2} a tu tarjeta.";
        return RedirectToAction(nameof(PayCreditCardSuccess));
    }

    public IActionResult PayCreditCardSuccess() => View();
}