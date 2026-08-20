using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ArtemisBankingPro.Core.Application.Features.Card.Commands.UpdateCredit
{
    public class UpdateCreditLimitCommandHandler : IRequestHandler<UpdateCreditLimitCommand, ListCreditCardDto>
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCreditLimitCommandHandler> _logger;

        public UpdateCreditLimitCommandHandler(
            ICreditCardRepository creditCardRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            IMapper mapper,
            ILogger<UpdateCreditLimitCommandHandler> logger)
        {
            _creditCardRepository = creditCardRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ListCreditCardDto> Handle(UpdateCreditLimitCommand request, CancellationToken cancellationToken)
        {
            var card = await _creditCardRepository.GetByIdAsync(request.CardId);
            if (card is null)
                throw new KeyNotFoundException("La tarjeta seleccionada no existe.");

            if (card.Status == CardStatus.Cancelled)
                throw new Exception("No se puede modificar una tarjeta cancelada.");

            if (request.CreditLimit < card.CurrentDebt)
                throw new Exception("El límite de la tarjeta no puede ser inferior al monto adeudado actualmente.");

            card.CreditLimit = request.CreditLimit;
            card.ModifiedAt = DateTime.UtcNow;

            await _creditCardRepository.UpdateAsync(card);

            var user = await _userRepository.GetByIdAsync(card.UserId);
            if (user != null)
            {
                try
                {
                    var last4 = card.CardNumber.Length >= 4 ? card.CardNumber[^4..] : card.CardNumber;
                    var body = $@"Hola {user.FirstName} {user.LastName},<br><br>
                                El límite de su tarjeta de crédito terminada en {last4} ha sido actualizado.<br><br>
                                <b>Nuevo límite aprobado:</b> RD${card.CreditLimit:N2}<br>
                                <b>Fecha de modificación:</b> {card.ModifiedAt:dd/MM/yyyy}<br><br>
                                Si usted no reconoce esta modificación, comuníquese con la entidad bancaria.";

                    await _emailService.SendEmailAsync(user.Email, "Modificación de límite de tarjeta", body);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al enviar correo de notificación por modificación de límite");
                }
            }

            return _mapper.Map<ListCreditCardDto>(card);
        }
    }
}