using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ArtemisBankingPro.Core.Application.Features.Card.Commands.AssignCreditCard
{
    public class AssignCreditCardCommandHandler : IRequestHandler<AssignCreditCardCommand, CreateCreditCardResponseDto>
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICardNumberGeneratorService _cardNumberGenerator;
        private readonly ICvcHashService _cvcHashService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<AssignCreditCardCommandHandler> _logger;

        public AssignCreditCardCommandHandler(
            ICreditCardRepository creditCardRepository,
            IUserRepository userRepository,
            ICardNumberGeneratorService cardNumberGenerator,
            ICvcHashService cvcHashService,
            IEmailService emailService,
            IMapper mapper,
            ILogger<AssignCreditCardCommandHandler> logger)
        {
            _creditCardRepository = creditCardRepository;
            _userRepository = userRepository;
            _cardNumberGenerator = cardNumberGenerator;
            _cvcHashService = cvcHashService;
            _emailService = emailService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateCreditCardResponseDto> Handle(AssignCreditCardCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.ClientId);
            if (user is null)
                throw new KeyNotFoundException("El cliente seleccionado no existe.");

            if (!user.IsActive)
                throw new Exception("Solo se puede asignar tarjetas de crédito a clientes activos.");

            var cardNumber = await _cardNumberGenerator.GenerateUniqueCardNumberAsync();
            var (_, hashedCvc) = _cvcHashService.GenerateHashedCvc();
            var now = DateTime.UtcNow;
            var expirationDate = now.AddYears(3);

            var creditCard = new CreditCard
            {
                UserId = request.ClientId,
                CardNumber = cardNumber,
                ExpirationDate = expirationDate,
                CvcHash = hashedCvc,
                CreditLimit = request.CreditLimit,
                CurrentDebt = 0.00m,
                Status = CardStatus.Active,
                CreatedAt = now,
                ModifiedAt = now
            };

            await _creditCardRepository.AddAsync(creditCard);

            var savedCard = await _creditCardRepository.GetByCardNumberAsync(cardNumber);
            var response = _mapper.Map<CreateCreditCardResponseDto>(savedCard);

            try
            {
                var body = $@"Hola {user.FirstName} {user.LastName},<br><br>
                            Se ha asignado una nueva tarjeta de crédito a su cuenta.<br><br>
                            <b>Tarjeta terminada en:</b> {response.LastFourDigits}<br>
                            <b>Límite aprobado:</b> RD${response.CreditLimit:N2}<br>
                            <b>Fecha de expiración:</b> {response.ExpirationDate}<br>
                            <b>Fecha de asignación:</b> {now:dd/MM/yyyy}<br><br>
                            Por seguridad, no comparta la información de su tarjeta con terceros.";

                await _emailService.SendEmailAsync(user.Email, "Nueva tarjeta de crédito asignada", body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo de notificación al cliente {UserId}", user.Id);
            }

            return response;
        }
    }
}