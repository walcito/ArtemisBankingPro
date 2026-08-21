using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;
using ArtemisBankingPro.Core.Application.Dtos.SavingsAccount;
using ArtemisBankingPro.Core.Application.Dtos.SavingsAccount;
using ArtemisBankingPro.Core.Application.ViewModels.SavingsAccount;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetAccountTransactions;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetSavingsAccounts;

namespace ArtemisBankingPro.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class SavingsAccountController : Controller
{
    private readonly IMediator _mediator;

    public SavingsAccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(
        int pageNumber = 1, int pageSize = 10, string? identificationNumber = null,
        AccountStatus? status = null,
        AccountType? type = null)
    {
        var query = new GetSavingsAccountsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IdentificationNumber = identificationNumber,
            Status = status,
            Type = type
        };

        var result = await _mediator.Send(query);

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
        var query = new GetAccountTransactionsQuery
        {
            AccountNumber = accountNumber,
            PageNumber = pageNumber,
            PageSize = 10
        };

        var result = await _mediator.Send(query);
        
        var accounts = await _mediator.Send(new GetSavingsAccountsQuery { PageSize = 1000 });
        var account = accounts.Items.FirstOrDefault(a => a.AccountNumber == accountNumber);

        var vm = new AccountDetailsViewModel
        {
            AccountNumber = accountNumber,
            Balance = account?.Balance ?? 0m,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            Transactions = result.Items.Select(t => new TransactionViewModel
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
        var users = await _mediator.Send(new ArtemisBankingPro.Core.Application.Features.Users.GetUsersQuery());
        // Filtrar clientes activos
        var clients = users.Where(u => u.Role == "Cliente" && u.IsActive).ToList();
        
        if (!string.IsNullOrEmpty(identificationNumber))
        {
            clients = clients.Where(c => c.Identification.Contains(identificationNumber)).ToList();
        }

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
            return await Create(vm.UserId == 0 ? (int?)null : vm.UserId); // Re-render with errors
        }

        var command = new CreateSavingsAccountCommand
        {
            UserId = vm.UserId,
            InitialAmount = vm.InitialAmount
        };
        await _mediator.Send(command);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(string accountNumber)
    {
        await _mediator.Send(new CancelSavingsAccountCommand { AccountNumber = accountNumber });
        return RedirectToAction(nameof(Index));
    }
}