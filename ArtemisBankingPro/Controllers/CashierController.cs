using System.Security.Claims;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.ThirdPartyTransaction;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayLoan;
using ArtemisBankingPro.Core.Application.Dtos.Cashier;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Deposit;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Withdraw;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayCreditCard;
using ArtemisBankingPro.Core.Application.ViewModels.Cashier;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
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
        if (!string.IsNullOrEmpty(idClaim) && int.TryParse(idClaim, out var cashierId))
            return cashierId;

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
        var query = new GetCashierHomeQuery { CashierUserId = GetAuthenticatedCashierId() };
        var result = await _mediator.Send(query);
        var vm = new CashierHomeViewModel
        {
            TotalTransactionsToday = result.TotalTransactionsToday,
            TotalAmountProcessedToday = result.TotalAmountProcessedToday,
            RejectedTransactionsToday = result.RejectedTransactionsToday
        };
        return View(vm);
    }

    public IActionResult Deposit() => View(new DepositViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Deposit(DepositViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        // Guardamos el ViewModel temporalmente, para reutilizarlo en la pantalla de confirmación
        TempData["PendingDeposit"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmDeposit));
    }

    public IActionResult ConfirmDeposit()
    {
        if (TempData.Peek("PendingDeposit") is not string json)
            return RedirectToAction(nameof(Deposit));

        var vm = JsonSerializer.Deserialize<DepositViewModel>(json);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDeposit(bool confirmed)
    {
        if (TempData["PendingDeposit"] is not string json)
            return RedirectToAction(nameof(Deposit));

        var vm = JsonSerializer.Deserialize<DepositViewModel>(json);

        if (!confirmed || vm is null)
            return RedirectToAction(nameof(Deposit));

        var command = new DepositCommand
        {
            AccountNumber = vm.AccountNumber,
            Amount = vm.Amount
        };
        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se acreditaron {vm.Amount:C2} a la cuenta {vm.AccountNumber}.";
        return RedirectToAction(nameof(DepositSuccess));
    }

    public IActionResult DepositSuccess() => View();

    public IActionResult Withdraw() => View(new WithdrawViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Withdraw(WithdrawViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        TempData["PendingWithdraw"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmWithdraw));
    }

    public IActionResult ConfirmWithdraw()
    {
        if (TempData.Peek("PendingWithdraw") is not string json)
            return RedirectToAction(nameof(Withdraw));

        var vm = JsonSerializer.Deserialize<WithdrawViewModel>(json);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmWithdraw(bool confirmed)
    {
        if (TempData["PendingWithdraw"] is not string json)
            return RedirectToAction(nameof(Withdraw));

        var vm = JsonSerializer.Deserialize<WithdrawViewModel>(json);

        if (!confirmed || vm is null)
            return RedirectToAction(nameof(Withdraw));

        var command = new WithdrawCommand
        {
            AccountNumber = vm.AccountNumber,
            Amount = vm.Amount
        };
        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se descontaron {vm.Amount:C2} de la cuenta {vm.AccountNumber}.";
        return RedirectToAction(nameof(WithdrawSuccess));
    }

    public IActionResult WithdrawSuccess() => View();

    public IActionResult ThirdPartyTransaction() => View(new ThirdPartyTransactionViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ThirdPartyTransaction(ThirdPartyTransactionViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        TempData["PendingThirdParty"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmThirdPartyTransaction));
    }

    public IActionResult ConfirmThirdPartyTransaction()
    {
        if (TempData.Peek("PendingThirdParty") is not string json)
            return RedirectToAction(nameof(ThirdPartyTransaction));

        var vm = JsonSerializer.Deserialize<ThirdPartyTransactionViewModel>(json);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmThirdPartyTransaction(bool confirmed)
    {
        if (TempData["PendingThirdParty"] is not string json)
            return RedirectToAction(nameof(ThirdPartyTransaction));

        var vm = JsonSerializer.Deserialize<ThirdPartyTransactionViewModel>(json);

        if (!confirmed || vm is null)
            return RedirectToAction(nameof(ThirdPartyTransaction));

        var command = new ThirdPartyTransactionCommand
        {
            SourceAccountNumber = vm.SourceAccountNumber,
            DestinationAccountNumber = vm.DestinationAccountNumber,
            Amount = vm.Amount,
            CashierUserId = GetAuthenticatedCashierId()
        };
        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se transfirieron {vm.Amount:C2} desde la cuenta {vm.SourceAccountNumber} hacia la cuenta {vm.DestinationAccountNumber}.";
        return RedirectToAction(nameof(ThirdPartyTransactionSuccess));
    }

    public IActionResult ThirdPartyTransactionSuccess() => View();
    public IActionResult PayLoan() => View(new CashierPayLoanViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PayLoan(CashierPayLoanViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        TempData["PendingPayLoan"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmPayLoan));
    }

    public IActionResult ConfirmPayLoan()
    {
        if (TempData.Peek("PendingPayLoan") is not string json)
            return RedirectToAction(nameof(PayLoan));

        var vm = JsonSerializer.Deserialize<CashierPayLoanViewModel>(json);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPayLoan(bool confirmed)
    {
        if (TempData["PendingPayLoan"] is not string json)
            return RedirectToAction(nameof(PayLoan));

        var vm = JsonSerializer.Deserialize<CashierPayLoanViewModel>(json);

        if (!confirmed || vm is null)
            return RedirectToAction(nameof(PayLoan));

        var command = new PayLoanCommand
        {
            SourceAccountNumber = vm.SourceAccountNumber,
            LoanId = vm.LoanId,
            Amount = vm.Amount,
            CashierUserId = GetAuthenticatedCashierId()
        };
        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se aplicó un abono de {vm.Amount:C2} al préstamo #{vm.LoanId}.";
        return RedirectToAction(nameof(PayLoanSuccess));
    }

    public IActionResult PayLoanSuccess() => View();

    public IActionResult PayCreditCard() => View(new CashierPayCreditCardViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult PayCreditCard(CashierPayCreditCardViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        TempData["PendingPayCreditCard"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmPayCreditCard));
    }

    public IActionResult ConfirmPayCreditCard()
    {
        if (TempData.Peek("PendingPayCreditCard") is not string json)
            return RedirectToAction(nameof(PayCreditCard));

        var vm = JsonSerializer.Deserialize<CashierPayCreditCardViewModel>(json);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPayCreditCard(bool confirmed)
    {
        if (TempData["PendingPayCreditCard"] is not string json)
            return RedirectToAction(nameof(PayCreditCard));

        var vm = JsonSerializer.Deserialize<CashierPayCreditCardViewModel>(json);

        if (!confirmed || vm is null)
            return RedirectToAction(nameof(PayCreditCard));

        var command = new PayCreditCardCommand
        {
            SourceAccountNumber = vm.SourceAccountNumber,
            CreditCardId = vm.CreditCardId,
            Amount = vm.Amount,
            CashierUserId = GetAuthenticatedCashierId()
        };
        await _mediator.Send(command);
        TempData["SuccessMessage"] = $"Se aplicó un pago de {vm.Amount:C2} a la tarjeta #{vm.CreditCardId}.";
        return RedirectToAction(nameof(PayCreditCardSuccess));
    }

    public IActionResult PayCreditCardSuccess() => View();
}