using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Application.Features.Card.Commands.AssignCreditCard;
using ArtemisBankingPro.Core.Application.Features.Card.Commands.CancelCreditCard;
using ArtemisBankingPro.Core.Application.Features.Card.Commands.UpdateCredit;
using ArtemisBankingPro.Core.Application.Features.Card.Queries.GetCreditCards;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingProApi.Controllers
{
    [ApiController]
    [Route("api/credit-card")]
    [Authorize(Roles = "Administrador")]
    public class CreditCardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CreditCardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] string? identification = null)
        {
            CardStatus? parsedStatus = null;
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.Equals("activa", StringComparison.OrdinalIgnoreCase))
                    parsedStatus = CardStatus.Active;
                else if (status.Equals("cancelada", StringComparison.OrdinalIgnoreCase))
                    parsedStatus = CardStatus.Cancelled;
            }

            var query = new GetCreditCardsQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = parsedStatus,
                Identification = identification
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AssignCard([FromBody] AssignCreditCardCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetCreditCardByIdQuery { CardId = id });
            return Ok(result);
        }

        [HttpPatch("{id:int}/limit")]
        public async Task<IActionResult> UpdateLimit(int id, [FromBody] UpdateCreditLimitRequestDto dto)
        {
            await _mediator.Send(new UpdateCreditLimitCommand
            {
                CardId = id,
                CreditLimit = dto.CreditLimit
            });
            return NoContent();
        }

        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> CancelCard(int id)
        {
            await _mediator.Send(new CancelCreditCardCommand { CardId = id });
            return NoContent();
        }
    }
}
