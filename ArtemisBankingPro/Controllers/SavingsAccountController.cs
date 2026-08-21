using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.ViewModels.SavingsAccount;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingPro.WebApp.Controllers;

[Authorize(Roles = "Admin , Administrador")]
public class SavingsAccountController : Controller
{
    private readonly ISavingsAccountService _savingsAccountService;

    public SavingsAccountController(ISavingsAccountService savingsAccountService)
    {
        _savingsAccountService = savingsAccountService;
    }

    public async Task<IActionResult> Index(
        int pageNumber = 1, int pageSize = 10, string? identificationNumber = null,
        AccountStatus? status = null,
        AccountType? type = null)
    {
        var result = await _savingsAccountService.GetSavingsAccountsAsync(pageNumber, pageSize, identificationNumber, status, type);

        var vm = new SavingsAccountListViewModel
        {
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            Items = result.Items.Select(a => new SavingsAccountViewModel
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                Balance = a.Balance,
                Type = a.Type,
                Status = a.Status,
                UserFullName = a.UserFullName,
                UserIdentificationNumber = a.UserIdentificationNumber
            }).ToList()
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(string accountNumber, int pageNumber = 1)
    {
        var transResult = await _savingsAccountService.GetAccountTransactionsAsync(accountNumber, pageNumber, 10);
        
        var accounts = await _savingsAccountService.GetSavingsAccountsAsync(1, 1000, null, null, null);
        var account = accounts.Items.FirstOrDefault(a => a.AccountNumber == accountNumber);

        var vm = new AccountDetailsViewModel
        {
            AccountNumber = accountNumber,
            Balance = account?.Balance ?? 0m,
            TotalCount = transResult.TotalCount,
            PageNumber = transResult.PageNumber,
            PageSize = transResult.PageSize,
            Transactions = transResult.Items.Select(t => new TransactionViewModel
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                Date = t.Date,
                Description = t.Description
            }).ToList()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? selectedUserId = null, string? identificationNumber = null)
    {
        var clients = await _savingsAccountService.GetActiveClientsAsync(identificationNumber);

        ViewBag.Clients = clients;
        ViewBag.SelectedUserId = selectedUserId;
        
        if (selectedUserId.HasValue)
        {
            var selectedUser = clients.FirstOrDefault(c => c.Id == selectedUserId.Value.ToString());
            ViewBag.SelectedUser = selectedUser;
        }

        return View(new CreateSavingsAccountViewModel { UserId = selectedUserId ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSavingsAccountViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return await Create(vm.UserId == 0 ? (int?)null : vm.UserId);
        }

        await _savingsAccountService.CreateSavingsAccountAsync(vm.UserId, vm.InitialAmount);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(string accountNumber)
    {
        await _savingsAccountService.CancelSavingsAccountAsync(accountNumber);
        return RedirectToAction(nameof(Index));
    }
}