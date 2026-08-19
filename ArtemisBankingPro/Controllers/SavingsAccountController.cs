using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetSavingsAccounts;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetAccountTransactions;
using ArtemisBankingPro.Core.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        ArtemisBankingPro.Core.Domain.Common.Enums.AccountStatus? status = null,
        ArtemisBankingPro.Core.Domain.Common.Enums.AccountType? type = null)
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
        return View(result);
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
        ViewBag.AccountNumber = accountNumber;
        
        var accounts = await _mediator.Send(new GetSavingsAccountsQuery { PageSize = 1000 });
        var account = accounts.Items.FirstOrDefault(a => a.AccountNumber == accountNumber);
        ViewBag.Balance = account?.Balance ?? 0m;
        
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Create(string? selectedUserId = null, string? identificationNumber = null)
    {
        var users = await _mediator.Send(new ArtemisBankingPro.Core.Application.Features.Users.GetUsersQuery());
        // Filtrar clientes activos
        var clients = users.Where(u => u.Role == "Cliente" && u.IsActive).ToList();
        
        if (!string.IsNullOrEmpty(identificationNumber))
        {
            clients = clients.Where(c => c.IdentificationNumber.Contains(identificationNumber)).ToList();
        }

        ViewBag.Clients = clients;
        ViewBag.SelectedUserId = selectedUserId;
        
        if (!string.IsNullOrEmpty(selectedUserId))
        {
            var selectedUser = clients.FirstOrDefault(c => c.Id == selectedUserId);
            ViewBag.SelectedUser = selectedUser;
        }

        return View(new CreateSavingsAccountCommand { UserId = selectedUserId ?? "" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSavingsAccountCommand command)
    {
        if (!ModelState.IsValid)
        {
            return await Create(command.UserId); // Re-render with errors
        }

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