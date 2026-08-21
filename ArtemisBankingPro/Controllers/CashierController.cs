using System.Security.Claims;
using ArtemisBankingPro.Core.Application.Interfaces.Cashier;
using ArtemisBankingPro.Core.Application.ViewModels.Cashier;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBankingPro.WebApp.Controllers;

[Authorize(Roles = "Cashier,Cajero,Teller")]
public class CashierController : Controller
{
    private readonly ICashierService _cashierService;

    public CashierController(ICashierService cashierService)
    {
        _cashierService = cashierService;
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
        var result = await _cashierService.GetCashierHomeAsync(GetAuthenticatedCashierId());
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

        await _cashierService.DepositAsync(vm.AccountNumber, vm.Amount);
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

        await _cashierService.WithdrawAsync(vm.AccountNumber, vm.Amount);
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

        await _cashierService.ThirdPartyTransactionAsync(vm.SourceAccountNumber, vm.DestinationAccountNumber, vm.Amount, GetAuthenticatedCashierId());
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

        await _cashierService.PayLoanAsync(vm.SourceAccountNumber, vm.LoanId, vm.Amount, GetAuthenticatedCashierId());
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

        await _cashierService.PayCreditCardAsync(vm.SourceAccountNumber, vm.CreditCardId, vm.Amount, GetAuthenticatedCashierId());
        TempData["SuccessMessage"] = $"Se aplicó un pago de {vm.Amount:C2} a la tarjeta #{vm.CreditCardId}.";
        return RedirectToAction(nameof(PayCreditCardSuccess));
    }

    public IActionResult PayCreditCardSuccess() => View();
}