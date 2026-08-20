using ArtemisBankingPro.Core.Application.Dtos.Card;
using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.Users;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ArtemisBankingPro.Core.Application.Services.Card
{
    public class CreditCardService : GenericService<CreditCard>, ICreditCardService
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccountService _accountService;
        private readonly ICardNumberGeneratorService _cardNumberGenerator;
        private readonly ICvcHashService _cvcHashService;
        private readonly IEmailService _emailService;
        private readonly ILogger<CreditCardService> _logger;

        public CreditCardService(
            ICreditCardRepository creditCardRepository,
            IUserRepository userRepository,
            IAccountService accountService,
            ICardNumberGeneratorService cardNumberGenerator,
            ICvcHashService cvcHashService,
            IEmailService emailService,
            IMapper mapper,
            ILogger<CreditCardService> logger)
            : base(creditCardRepository, mapper)
        {
            _creditCardRepository = creditCardRepository;
            _userRepository = userRepository;
            _accountService = accountService;
            _cardNumberGenerator = cardNumberGenerator;
            _cvcHashService = cvcHashService;
            _emailService = emailService;
            _logger = logger;
        }

        public override async Task<IEnumerable<TDto>> GetAllAsync<TDto>()
        {
            var entities = await _creditCardRepository.GetAllWithUserAsync();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public async Task<CreditCardPagedResultDto> GetFilteredAsync(CreditCardFilterDto filter)
        {
            var allCards = await _creditCardRepository.GetAllWithUserAsync();
            var query = allCards.AsQueryable();
            if (filter.Status.HasValue)
                query = query.Where(c => c.Status == filter.Status.Value);
            if (!string.IsNullOrWhiteSpace(filter.Identification))
                query = query.Where(c => c.User != null && c.User.IdentificationNumber == filter.Identification.Trim());
            var totalRecords = query.Count();
            var paged = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
            return new CreditCardPagedResultDto
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                Data = _mapper.Map<List<ListCreditCardDto>>(paged)
            };
        }

        public async Task<DetailCreditCardDto?> GetDetailAsync(int id)
        {
            var card = await _creditCardRepository.GetByIdWithConsumptionsAsync(id);
            if (card is null) return null;
            return _mapper.Map<DetailCreditCardDto>(card);
        }

        public async Task<(bool Success, string Message, CreateCreditCardResponseDto? Data)> AssignCardAsync(CreateCreditCardDto dto)
        {
            User? user = null;
            if (int.TryParse(dto.ClientId, out int numericId))
            {
                user = await _userRepository.GetByIdAsync(numericId);
            }

            if (user == null)
            {
                var identityUser = await _accountService.GetUserByIdAsync(dto.ClientId);
                if (identityUser == null)
                    return (false, "El cliente especificado no existe.", null);

                if (!identityUser.IsActive)
                    return (false, "Solo se puede asignar tarjetas de crédito a clientes activos.", null);

                var allDomainUsers = await _userRepository.GetAllAsync();
                user = allDomainUsers.FirstOrDefault(u => u.Email == identityUser.Email);

                if (user == null)
                {
                    user = new User
                    {
                        FirstName = identityUser.FirstName,
                        LastName = identityUser.LastName,
                        IdentificationNumber = identityUser.Identification ?? string.Empty,
                        Email = identityUser.Email,
                        Username = identityUser.UserName,
                        Role = Roles.Client,
                        IsActive = true
                    };
                    await _userRepository.AddAsync(user);
                }
            }

            if (!user.IsActive)
                return (false, "Solo se puede asignar tarjetas de crédito a clientes activos.", null);

            if (dto.CreditLimit <= 0)
                return (false, "El límite de crédito debe ser mayor que cero.", null);

            var cardNumber = await _cardNumberGenerator.GenerateUniqueCardNumberAsync();
            var (_, hashedCvc) = _cvcHashService.GenerateHashedCvc();
            var now = DateTime.UtcNow;
            var expirationDate = now.AddYears(3);

            var card = new CreditCard
            {
                UserId = user.Id,
                CardNumber = cardNumber,
                ExpirationDate = expirationDate,
                CvcHash = hashedCvc,
                CreditLimit = dto.CreditLimit,
                CurrentDebt = 0.00m,
                Status = CardStatus.Active,
                CreatedAt = now,
                ModifiedAt = now
            };

            await _creditCardRepository.AddAsync(card);
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
                _logger.LogError(ex, "Error al enviar correo de asignación de tarjeta para el usuario {UserId}", user.Id);
            }

            return (true, "Tarjeta de crédito asignada exitosamente.", response);
        }

        public async Task<(bool Success, string Message, ListCreditCardDto? Data)> UpdateCreditLimitAsync(int cardId, UpdateCreditLimitRequestDto dto)
        {
            var card = await _creditCardRepository.GetByIdAsync(cardId);
            if (card is null)
                return (false, "La tarjeta seleccionada no existe.", null);
            if (card.Status == CardStatus.Cancelled)
                return (false, "No se puede modificar una tarjeta cancelada.", null);
            if (dto.CreditLimit <= 0)
                return (false, "El límite de la tarjeta debe ser mayor que cero.", null);
            if (dto.CreditLimit < card.CurrentDebt)
                return (false, "El límite de la tarjeta no puede ser inferior al monto adeudado actualmente.", null);

            card.CreditLimit = dto.CreditLimit;
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
                    _logger.LogError(ex, "Error al enviar correo de modificación de límite para tarjeta {CardId}", card.Id);
                }
            }
            return (true, "Límite de crédito actualizado correctamente.", _mapper.Map<ListCreditCardDto>(card));
        }

        public async Task<(bool Success, string Message)> CancelCardAsync(int cardId, string? reason)
        {
            var card = await _creditCardRepository.GetByIdAsync(cardId);
            if (card is null)
                return (false, "La tarjeta seleccionada no existe.");
            if (card.Status == CardStatus.Cancelled)
                return (false, "La tarjeta ya se encuentra cancelada.");
            if (card.CurrentDebt > 0)
                return (false, "Para cancelar esta tarjeta, el cliente debe saldar la totalidad de la deuda pendiente.");

            card.Status = CardStatus.Cancelled;
            card.CancelledAt = DateTime.UtcNow;
            card.ModifiedAt = DateTime.UtcNow;
            await _creditCardRepository.UpdateAsync(card);
            return (true, "Tarjeta de crédito cancelada exitosamente.");
        }
    }
}
