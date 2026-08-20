using ArtemisBankingPro.Core.Application.Features.HermesPay.Commands.ProcessPayment;
using ArtemisBankingPro.Core.Application.Features.HermesPay.Queries.GetCommerceTransactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingProApi.Controllers
{
    [ApiController]
    [Route("pay")]
    [Authorize(Roles = "Administrador,Comercio")]
    public class PayController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PayController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-transactions/{commerceId:int}")]
        public async Task<IActionResult> GetTransactions(
            int commerceId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var targetCommerceId = ResolveCommerceId(commerceId);
            if (!targetCommerceId.HasValue)
                return Forbid();

            var query = new GetCommerceTransactionsQuery
            {
                CommerceId = targetCommerceId.Value,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("process-payment/{commerceId:int}")]
        public async Task<IActionResult> ProcessPayment(int commerceId, [FromBody] ProcessPaymentRequestDto dto)
        {
            var targetCommerceId = ResolveCommerceId(commerceId);
            if (!targetCommerceId.HasValue)
                return Forbid();

            var command = new ProcessPaymentCommand
            {
                CommerceId = targetCommerceId.Value,
                CardNumber = dto.CardNumber,
                MonthExpirationCard = dto.MonthExpirationCard,
                YearExpirationCard = dto.YearExpirationCard,
                Cvc = dto.Cvc,
                TransactionAmount = dto.TransactionAmount,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers.UserAgent.ToString()
            };

            var result = await _mediator.Send(command);

            if (!result.IsApproved)
            {
                return BadRequest(new { message = result.Message });
            }

            return NoContent();
        }

        private int? ResolveCommerceId(int routeCommerceId)
        {
            if (User.IsInRole("Administrador"))
                return routeCommerceId;

            var claimCommerceId = User.FindFirst("commerceId")?.Value
                               ?? User.FindFirst("CommerceId")?.Value;

            if (int.TryParse(claimCommerceId, out var id))
                return id;

            return null;
        }
    }

    public class ProcessPaymentRequestDto
    {
        public string CardNumber { get; set; } = string.Empty;
        public string MonthExpirationCard { get; set; } = string.Empty;
        public string YearExpirationCard { get; set; } = string.Empty;
        public string Cvc { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; }
    }
}