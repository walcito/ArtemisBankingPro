using System.Security.Claims;
using ArtemisBankingPro.Core.Application.Dtos.Client;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.ViewModels.Client;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBankingPro.WebApp.Controllers;

[Authorize(Roles = "Client,Cliente")]
public class ClientController : Controller
{
    private readonly IClientProductService _clientService;

    public ClientController(IClientProductService clientService)
    {
        _clientService = clientService;
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

    private ClientHomeViewModel MapToViewModel(ClientHomeDto dto)
    {
        return new ClientHomeViewModel
        {
            SavingsAccounts = dto.SavingsAccounts.Select(s => new ProductSummaryViewModel
            {
                Id = s.Id,
                Identifier = s.Identifier,
                RelevantAmount = s.RelevantAmount,
                Status = s.Status
            }).ToList(),
            Loans = dto.Loans.Select(l => new ProductSummaryViewModel
            {
                Id = l.Id,
                Identifier = l.Identifier,
                RelevantAmount = l.RelevantAmount,
                Status = l.Status
            }).ToList(),
            CreditCards = dto.CreditCards.Select(c => new ProductSummaryViewModel
            {
                Id = c.Id,
                Identifier = c.Identifier,
                RelevantAmount = c.RelevantAmount,
                Status = c.Status
            }).ToList()
        };
    }

    public async Task<IActionResult> Index()
    {
        var result = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
        return View(MapToViewModel(result));
    }

    public async Task<IActionResult> Beneficiaries()
    {
        var result = await _clientService.GetBeneficiariesAsync(GetAuthenticatedUserId());
        var vms = result.Select(b => new BeneficiaryViewModel
        {
            Id = b.Id,
            Alias = b.Alias,
            BeneficiaryAccountNumber = b.BeneficiaryAccountNumber
        }).ToList();
        return View(vms);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBeneficiary(AddBeneficiaryViewModel command)
    {
        await _clientService.AddBeneficiaryAsync(GetAuthenticatedUserId(), command.BeneficiaryAccountNumber, command.Alias);
        return RedirectToAction(nameof(Beneficiaries));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveBeneficiary(int id)
    {
        await _clientService.RemoveBeneficiaryAsync(id, GetAuthenticatedUserId());
        return RedirectToAction(nameof(Beneficiaries));
    }

    public async Task<IActionResult> TransferOwnAccounts()
    {
        var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
        ViewBag.Accounts = homeData.SavingsAccounts;
        return View(new TransferOwnAccountsViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TransferOwnAccounts(TransferOwnAccountsViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
            ViewBag.Accounts = homeData.SavingsAccounts;
            return View(vm);
        }

        TempData["PendingTransferOwn"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmTransferOwnAccounts));
    }

    public IActionResult ConfirmTransferOwnAccounts()
    {
        if (TempData.Peek("PendingTransferOwn") is not string json)
            return RedirectToAction(nameof(TransferOwnAccounts));

        return View(JsonSerializer.Deserialize<TransferOwnAccountsViewModel>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmTransferOwnAccounts(bool confirmed)
    {
        if (TempData["PendingTransferOwn"] is not string json)
            return RedirectToAction(nameof(TransferOwnAccounts));

        var vm = JsonSerializer.Deserialize<TransferOwnAccountsViewModel>(json);
        if (!confirmed || vm is null) return RedirectToAction(nameof(TransferOwnAccounts));

        await _clientService.TransferBetweenOwnAccountsAsync(vm.SourceAccountNumber, vm.DestinationAccountNumber, vm.Amount, GetAuthenticatedUserId());
        TempData["SuccessMessage"] = $"Se transfirieron {vm.Amount:C2} entre tus cuentas propias.";
        return RedirectToAction(nameof(TransferOwnAccountsSuccess));
    }

    public IActionResult TransferOwnAccountsSuccess() => View();

    public async Task<IActionResult> ExpressTransaction()
    {
        var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
        ViewBag.Accounts = homeData.SavingsAccounts;
        return View(new ExpressTransactionViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExpressTransaction(ExpressTransactionViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
            ViewBag.Accounts = homeData.SavingsAccounts;
            return View(vm);
        }

        TempData["PendingExpress"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmExpressTransaction));
    }

    public IActionResult ConfirmExpressTransaction()
    {
        if (TempData.Peek("PendingExpress") is not string json)
            return RedirectToAction(nameof(ExpressTransaction));

        return View(JsonSerializer.Deserialize<ExpressTransactionViewModel>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmExpressTransaction(bool confirmed)
    {
        if (TempData["PendingExpress"] is not string json)
            return RedirectToAction(nameof(ExpressTransaction));

        var vm = JsonSerializer.Deserialize<ExpressTransactionViewModel>(json);
        if (!confirmed || vm is null) return RedirectToAction(nameof(ExpressTransaction));

        await _clientService.ExpressTransactionAsync(vm.SourceAccountNumber, vm.DestinationAccountNumber, vm.Amount, GetAuthenticatedUserId());
        TempData["SuccessMessage"] = $"Se transfirieron {vm.Amount:C2} a la cuenta {vm.DestinationAccountNumber}.";
        return RedirectToAction(nameof(ExpressTransactionSuccess));
    }

    public IActionResult ExpressTransactionSuccess() => View();

    public async Task<IActionResult> TransferToBeneficiary()
    {
        var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
        ViewBag.Accounts = homeData.SavingsAccounts;
        var beneficiaries = await _clientService.GetBeneficiariesAsync(GetAuthenticatedUserId());
        ViewBag.Beneficiaries = beneficiaries;
        return View(new TransferToBeneficiaryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TransferToBeneficiary(TransferToBeneficiaryViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
            ViewBag.Accounts = homeData.SavingsAccounts;
            var beneficiaries = await _clientService.GetBeneficiariesAsync(GetAuthenticatedUserId());
            ViewBag.Beneficiaries = beneficiaries;
            return View(vm);
        }

        TempData["PendingTransferBeneficiary"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmTransferToBeneficiary));
    }

    public IActionResult ConfirmTransferToBeneficiary()
    {
        if (TempData.Peek("PendingTransferBeneficiary") is not string json)
            return RedirectToAction(nameof(TransferToBeneficiary));

        return View(JsonSerializer.Deserialize<TransferToBeneficiaryViewModel>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmTransferToBeneficiary(bool confirmed)
    {
        if (TempData["PendingTransferBeneficiary"] is not string json)
            return RedirectToAction(nameof(TransferToBeneficiary));

        var vm = JsonSerializer.Deserialize<TransferToBeneficiaryViewModel>(json);
        if (!confirmed || vm is null) return RedirectToAction(nameof(TransferToBeneficiary));

        await _clientService.TransferToBeneficiaryAsync(vm.SourceAccountNumber, vm.BeneficiaryId, vm.Amount, GetAuthenticatedUserId());
        TempData["SuccessMessage"] = $"Se transfirieron {vm.Amount:C2} a tu beneficiario.";
        return RedirectToAction(nameof(TransferToBeneficiarySuccess));
    }

    public IActionResult TransferToBeneficiarySuccess() => View();

    public async Task<IActionResult> CashAdvance()
    {
        var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
        ViewBag.Accounts = homeData.SavingsAccounts;
        ViewBag.Cards = homeData.CreditCards;
        return View(new CashAdvanceViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CashAdvance(CashAdvanceViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
            ViewBag.Accounts = homeData.SavingsAccounts;
            ViewBag.Cards = homeData.CreditCards;
            return View(vm);
        }

        TempData["PendingCashAdvance"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmCashAdvance));
    }

    public IActionResult ConfirmCashAdvance()
    {
        if (TempData.Peek("PendingCashAdvance") is not string json)
            return RedirectToAction(nameof(CashAdvance));

        return View(JsonSerializer.Deserialize<CashAdvanceViewModel>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmCashAdvance(bool confirmed)
    {
        if (TempData["PendingCashAdvance"] is not string json)
            return RedirectToAction(nameof(CashAdvance));

        var vm = JsonSerializer.Deserialize<CashAdvanceViewModel>(json);
        if (!confirmed || vm is null) return RedirectToAction(nameof(CashAdvance));

        await _clientService.CashAdvanceAsync(vm.CreditCardId, vm.DestinationAccountNumber, vm.Amount, GetAuthenticatedUserId());
        TempData["SuccessMessage"] = $"Se depositaron {vm.Amount:C2} en tu cuenta.";
        return RedirectToAction(nameof(CashAdvanceSuccess));
    }

    public IActionResult CashAdvanceSuccess() => View();

    public async Task<IActionResult> PayLoan()
    {
        var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
        ViewBag.Accounts = homeData.SavingsAccounts;
        ViewBag.Loans = homeData.Loans;
        return View(new PayLoanViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayLoan(PayLoanViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
            ViewBag.Accounts = homeData.SavingsAccounts;
            ViewBag.Loans = homeData.Loans;
            return View(vm);
        }

        TempData["PendingPayLoan"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmPayLoan));
    }

    public IActionResult ConfirmPayLoan()
    {
        if (TempData.Peek("PendingPayLoan") is not string json)
            return RedirectToAction(nameof(PayLoan));

        return View(JsonSerializer.Deserialize<PayLoanViewModel>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPayLoan(bool confirmed)
    {
        if (TempData["PendingPayLoan"] is not string json)
            return RedirectToAction(nameof(PayLoan));

        var vm = JsonSerializer.Deserialize<PayLoanViewModel>(json);
        if (!confirmed || vm is null) return RedirectToAction(nameof(PayLoan));

        await _clientService.PayLoanAsync(vm.SourceAccountNumber, vm.LoanId, vm.Amount, GetAuthenticatedUserId());
        TempData["SuccessMessage"] = $"Se aplicó un abono de {vm.Amount:C2} a tu préstamo.";
        return RedirectToAction(nameof(PayLoanSuccess));
    }

    public IActionResult PayLoanSuccess() => View();

    public async Task<IActionResult> PayCreditCard()
    {
        var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
        ViewBag.Accounts = homeData.SavingsAccounts;
        ViewBag.Cards = homeData.CreditCards;
        return View(new PayCreditCardViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayCreditCard(PayCreditCardViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var homeData = await _clientService.GetClientHomeAsync(GetAuthenticatedUserId());
            ViewBag.Accounts = homeData.SavingsAccounts;
            ViewBag.Cards = homeData.CreditCards;
            return View(vm);
        }

        TempData["PendingPayCreditCard"] = JsonSerializer.Serialize(vm);
        return RedirectToAction(nameof(ConfirmPayCreditCard));
    }

    public IActionResult ConfirmPayCreditCard()
    {
        if (TempData.Peek("PendingPayCreditCard") is not string json)
            return RedirectToAction(nameof(PayCreditCard));

        return View(JsonSerializer.Deserialize<PayCreditCardViewModel>(json));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmPayCreditCard(bool confirmed)
    {
        if (TempData["PendingPayCreditCard"] is not string json)
            return RedirectToAction(nameof(PayCreditCard));

        var vm = JsonSerializer.Deserialize<PayCreditCardViewModel>(json);
        if (!confirmed || vm is null) return RedirectToAction(nameof(PayCreditCard));

        await _clientService.PayCreditCardAsync(vm.SourceAccountNumber, vm.CreditCardId, vm.Amount, GetAuthenticatedUserId());
        TempData["SuccessMessage"] = $"Se aplicó un pago de {vm.Amount:C2} a tu tarjeta.";
        return RedirectToAction(nameof(PayCreditCardSuccess));
    }

    public IActionResult PayCreditCardSuccess() => View();
}