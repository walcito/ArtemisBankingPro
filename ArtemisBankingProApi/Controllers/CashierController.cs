using System.Security.Claims;
using ArtemisBankingPro.Core.Application.Dtos.Cashier;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayCreditCard;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.PayLoan;
using ArtemisBankingPro.Core.Application.Features.Cashier.Commands.ThirdPartyTransaction;
using ArtemisBankingPro.Core.Application.Features.Cashier.Queries.GetCashierHome;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Deposit;
using ArtemisBankingPro.Core.Application.Features.SavingsAccounts.Commands.Withdraw;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingProApi.Controllers;

[ApiController]
[Route("api/cashier")]
[Authorize(Roles = "Cashier,Cajero,Teller,Admin,Administrador")]
public class CashierController : ControllerBase
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
            return 1; // Default fallback to system cashier if claim is non-numeric

        return cashierId;
    }

    [HttpGet("home")]
    public async Task<IActionResult> GetHome()
    {
        var query = new GetCashierHomeQuery { CashierUserId = GetAuthenticatedCashierId() };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Depósito realizado correctamente." });
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Retiro realizado correctamente." });
    }

    [HttpPost("third-party-transaction")]
    public async Task<IActionResult> ThirdPartyTransaction([FromBody] ThirdPartyTransactionCommand command)
    {
        command.CashierUserId = GetAuthenticatedCashierId();
        await _mediator.Send(command);
        return Ok(new { message = "Transacción realizada correctamente." });
    }

    [HttpPost("pay-loan")]
    public async Task<IActionResult> PayLoan([FromBody] PayLoanCommand command)
    {
        command.CashierUserId = GetAuthenticatedCashierId();
        await _mediator.Send(command);
        return Ok(new { message = "Pago de préstamo procesado correctamente." });
    }

    [HttpPost("pay-credit-card")]
    public async Task<IActionResult> PayCreditCard([FromBody] PayCreditCardCommand command)
    {
        command.CashierUserId = GetAuthenticatedCashierId();
        await _mediator.Send(command);
        return Ok(new { message = "Pago de tarjeta de crédito procesado correctamente." });
    }
}