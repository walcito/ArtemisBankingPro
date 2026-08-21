using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CreateSavingsAccount;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.CancelSavingsAccount;
using ArtemisBankingPro.Core.Application.Dtos.SavingsAccount;
using ArtemisBankingPro.Core.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetAccountTransactions;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetSavingsAccounts;

namespace ArtemisBankingPro.WebAPI.Controllers;

[ApiController]
[Route("api/savings-account")]
[Authorize]
public class SavingsAccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public SavingsAccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetSavingsAccountsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSavingsAccountCommand command)
    {
        var newAccountId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = newAccountId }, newAccountId);
    }

    [HttpGet("{accountNumber}/transactions")]
    public async Task<IActionResult> GetTransactions(
        string accountNumber, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAccountTransactionsQuery
        {
            AccountNumber = accountNumber,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPatch("{accountNumber}/cancel")]
    public async Task<IActionResult> Cancel(string accountNumber)
    {
        await _mediator.Send(new CancelSavingsAccountCommand { AccountNumber = accountNumber });
        return NoContent();
    }
}