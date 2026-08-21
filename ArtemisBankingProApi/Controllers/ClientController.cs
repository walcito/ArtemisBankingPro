using System.Security.Claims;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.AddBeneficiary;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.RemoveBeneficiary;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferBetweenOwnAccounts;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.ExpressTransaction;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.TransferToBeneficiary;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.CashAdvance;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayLoanFromClient;
using ArtemisBankingPro.Core.Application.Features.Clients.Commands.PayCreditCardFromClient;
using ArtemisBankingPro.Core.Application.Dtos.Client;
using ArtemisBankingPro.Core.Application.Dtos.SavingsAccount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetBeneficiaries;
using ArtemisBankingPro.Core.Application.Features.Clients.Queries.GetClientHome;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Queries.GetAccountTransactions;

namespace ArtemisBankingPro.WebAPI.Controllers;

[ApiController]
[Route("api/client")]
[Authorize(Roles = "Client,Cliente")]
public class ClientController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private int GetAuthenticatedUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (idClaim is null || !int.TryParse(idClaim, out var userId))
            throw new Exception("No se pudo identificar al usuario autenticado.");

        return userId;
    }

    [HttpGet("home")]
    public async Task<IActionResult> GetHome()
    {
        var result = await _mediator.Send(new GetClientHomeQuery { UserId = GetAuthenticatedUserId() });
        return Ok(result);
    }

    [HttpGet("accounts/{accountNumber}/transactions")]
    public async Task<IActionResult> GetAccountTransactions(
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

    [HttpGet("beneficiaries")]
    public async Task<IActionResult> GetBeneficiaries()
    {
        var result = await _mediator.Send(new GetBeneficiariesQuery { OwnerUserId = GetAuthenticatedUserId() });
        return Ok(result);
    }

    [HttpPost("beneficiaries")]
    public async Task<IActionResult> AddBeneficiary([FromBody] AddBeneficiaryCommand command)
    {
        command.OwnerUserId = GetAuthenticatedUserId();
        var newId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBeneficiaries), new { id = newId }, newId);
    }

    [HttpDelete("beneficiaries/{id}")]
    public async Task<IActionResult> RemoveBeneficiary(int id)
    {
        await _mediator.Send(new RemoveBeneficiaryCommand { Id = id, OwnerUserId = GetAuthenticatedUserId() });
        return NoContent();
    }

    [HttpPost("transfer-own-accounts")]
    public async Task<IActionResult> TransferBetweenOwnAccounts([FromBody] TransferBetweenOwnAccountsCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("express-transaction")]
    public async Task<IActionResult> ExpressTransaction([FromBody] ExpressTransactionCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("transfer-beneficiary")]
    public async Task<IActionResult> TransferToBeneficiary([FromBody] TransferToBeneficiaryCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("cash-advance")]
    public async Task<IActionResult> CashAdvance([FromBody] CashAdvanceCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("pay-loan")]
    public async Task<IActionResult> PayLoan([FromBody] PayLoanFromClientCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("pay-credit-card")]
    public async Task<IActionResult> PayCreditCard([FromBody] PayCreditCardFromClientCommand command)
    {
        command.RequestingUserId = GetAuthenticatedUserId();
        await _mediator.Send(command);
        return NoContent();
    }

}