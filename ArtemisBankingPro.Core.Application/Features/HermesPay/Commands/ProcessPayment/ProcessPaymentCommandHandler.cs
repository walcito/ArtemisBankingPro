using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using ArtemisBankingPro.Core.Domain.Common.Enums.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Common.Enums.Transactions;
using ArtemisBankingPro.Core.Domain.Entities.Card;
using ArtemisBankingPro.Core.Domain.Entities.SavingsAccounts;
using ArtemisBankingPro.Core.Domain.Entities.Trades;
using ArtemisBankingPro.Core.Domain.Entities.Transactions;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Core.Domain.Interfaces.Card;
using ArtemisBankingPro.Core.Domain.Interfaces.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ArtemisBankingPro.Core.Application.Features.HermesPay.Commands.ProcessPayment
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, ProcessPaymentResponseDto>
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly ICardConsumptionRepository _cardConsumptionRepository;
        private readonly ICardProcessingLogRepository _cardProcessingLogRepository;
        private readonly IGenericRepository<Trade> _tradeRepository;
        private readonly IGenericRepository<SavingsAccount> _savingsAccountRepository;
        private readonly IGenericRepository<Transaction> _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICvcHashService _cvcHashService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ProcessPaymentCommandHandler> _logger;

        public ProcessPaymentCommandHandler(
            ICreditCardRepository creditCardRepository,
            ICardConsumptionRepository cardConsumptionRepository,
            ICardProcessingLogRepository cardProcessingLogRepository,
            IGenericRepository<Trade> tradeRepository,
            IGenericRepository<SavingsAccount> savingsAccountRepository,
            IGenericRepository<Transaction> transactionRepository,
            IUserRepository userRepository,
            ICvcHashService cvcHashService,
            IEmailService emailService,
            ILogger<ProcessPaymentCommandHandler> logger)
        {
            _creditCardRepository = creditCardRepository;
            _cardConsumptionRepository = cardConsumptionRepository;
            _cardProcessingLogRepository = cardProcessingLogRepository;
            _tradeRepository = tradeRepository;
            _savingsAccountRepository = savingsAccountRepository;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _cvcHashService = cvcHashService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ProcessPaymentResponseDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            var processedAt = DateTime.UtcNow;

            var commerce = await _tradeRepository.GetByIdAsync(request.CommerceId);
            if (commerce is null)
                throw new KeyNotFoundException("El comercio indicado no existe.");

            if (!commerce.IsActive)
                throw new Exception("El comercio indicado se encuentra inactivo.");

            if (!commerce.AssociatedUserId.HasValue)
                throw new Exception("El comercio no tiene un usuario asociado.");

            var allAccounts = await _savingsAccountRepository.GetAllAsync();
            var commerceMainAccount = allAccounts.FirstOrDefault(a =>
                a.UserId == commerce.AssociatedUserId.Value &&
                a.Type == AccountType.Principal &&
                a.Status == AccountStatus.Active);

            if (commerceMainAccount is null)
                throw new Exception("El usuario de comercio no posee una cuenta de ahorro principal activa.");

            var card = await _creditCardRepository.GetByCardNumberAsync(request.CardNumber);
            if (card is null)
                return await RegisterRejectionAsync(null, request, "La tarjeta ingresada no existe en el sistema.", processedAt);

            if (card.Status != CardStatus.Active)
                return await RegisterRejectionAsync(card.Id, request, "La tarjeta no se encuentra activa.", processedAt);

            if (card.ExpirationDate < processedAt)
                return await RegisterRejectionAsync(card.Id, request, "La tarjeta se encuentra vencida.", processedAt);

            var expMonth = int.Parse(request.MonthExpirationCard);
            var expYear = request.YearExpirationCard.Length == 2
                ? 2000 + int.Parse(request.YearExpirationCard)
                : int.Parse(request.YearExpirationCard);

            if (card.ExpirationDate.Month != expMonth || card.ExpirationDate.Year != expYear)
                return await RegisterRejectionAsync(card.Id, request, "La fecha de expiración de la tarjeta no coincide.", processedAt);

            if (!_cvcHashService.VerifyCvc(request.Cvc, card.CvcHash))
                return await RegisterRejectionAsync(card.Id, request, "El CVC ingresado es incorrecto.", processedAt);

            if (card.AvailableCredit < request.TransactionAmount)
                return await RegisterRejectionAsync(card.Id, request, "El monto de la transacción excede el crédito disponible de la tarjeta.", processedAt);

            // PAGO APROBADO 
            card.CurrentDebt += request.TransactionAmount;
            card.ModifiedAt = processedAt;
            await _creditCardRepository.UpdateAsync(card);

            var consumption = new CardConsumption
            {
                CardId = card.Id,
                CommerceId = commerce.Id,
                Amount = request.TransactionAmount,
                Status = ConsumptionStatus.Approved,
                TransactionDate = processedAt,
                Description = string.IsNullOrWhiteSpace(request.Description) ? commerce.Name : request.Description,
                CreatedAt = processedAt
            };
            await _cardConsumptionRepository.AddAsync(consumption);

            commerceMainAccount.Balance += request.TransactionAmount;
            await _savingsAccountRepository.UpdateAsync(commerceMainAccount);

            var last4 = card.CardNumber.Length >= 4 ? card.CardNumber[^4..] : card.CardNumber;
            var financialTx = new Transaction
            {
                Amount = request.TransactionAmount,
                Type = TransactionType.Credit,
                Status = TransactionStatus.Approved,
                Date = processedAt,
                SavingsAccountId = commerceMainAccount.Id
            };
            await _transactionRepository.AddAsync(financialTx);

            var log = new CardProcessingLog
            {
                CardId = card.Id,
                CommerceId = commerce.Id,
                RequestedAmount = request.TransactionAmount,
                ProcessingStatus = ProcessingStatus.Success,
                ProcessedAt = processedAt,
                ApprovedConsumptionId = consumption.Id,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                CreatedAt = processedAt
            };
            await _cardProcessingLogRepository.AddAsync(log);

            // Notificaciones por correo
            var clientUser = await _userRepository.GetByIdAsync(card.UserId);
            if (clientUser != null)
            {
                try
                {
                    var clientBody = $@"Hola {clientUser.FirstName} {clientUser.LastName},<br><br>
                                      Se ha realizado un consumo con su tarjeta terminada en {last4}.<br><br>
                                      <b>Comercio:</b> {commerce.Name}<br>
                                      <b>Monto:</b> RD${request.TransactionAmount:N2}<br>
                                      <b>Fecha y hora:</b> {processedAt:dd/MM/yyyy HH:mm:ss}<br><br>
                                      Si usted no reconoce esta operación, comuníquese con la entidad bancaria.";

                    await _emailService.SendEmailAsync(clientUser.Email, $"Consumo realizado con la tarjeta {last4}", clientBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al enviar correo de notificación al cliente del consumo");
                }
            }

            if (!string.IsNullOrWhiteSpace(commerce.Mail))
            {
                try
                {
                    var commerceBody = $@"Hola {commerce.Name},<br><br>
                                        Ha recibido un nuevo pago mediante Hermes Pay.<br><br>
                                        <b>Tarjeta terminada en:</b> {last4}<br>
                                        <b>Monto recibido:</b> RD${request.TransactionAmount:N2}<br>
                                        <b>Fecha y hora:</b> {processedAt:dd/MM/yyyy HH:mm:ss}<br><br>
                                        Este mensaje sirve como constancia del pago recibido.";

                    await _emailService.SendEmailAsync(commerce.Mail, $"Pago recibido a través de tarjeta {last4}", commerceBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al enviar correo de notificación al comercio");
                }
            }

            return new ProcessPaymentResponseDto
            {
                IsApproved = true,
                Message = "El pago fue procesado correctamente.",
                ConsumptionId = consumption.Id,
                MaskedCardNumber = $"************{last4}",
                ApprovedAmount = request.TransactionAmount,
                ProcessedAt = processedAt
            };
        }

        private async Task<ProcessPaymentResponseDto> RegisterRejectionAsync(
            int? cardId, ProcessPaymentCommand request, string reason, DateTime processedAt)
        {
            if (cardId.HasValue)
            {
                var rejectedConsumption = new CardConsumption
                {
                    CardId = cardId.Value,
                    CommerceId = request.CommerceId,
                    Amount = request.TransactionAmount,
                    Status = ConsumptionStatus.Rejected,
                    TransactionDate = processedAt,
                    Description = request.Description,
                    RejectionReason = reason,
                    CreatedAt = processedAt
                };
                await _cardConsumptionRepository.AddAsync(rejectedConsumption);

                var log = new CardProcessingLog
                {
                    CardId = cardId.Value,
                    CommerceId = request.CommerceId,
                    RequestedAmount = request.TransactionAmount,
                    ProcessingStatus = ProcessingStatus.Failed,
                    ValidationErrors = reason,
                    ProcessedAt = processedAt,
                    IpAddress = request.IpAddress,
                    UserAgent = request.UserAgent,
                    CreatedAt = processedAt
                };
                await _cardProcessingLogRepository.AddAsync(log);
            }

            var last4 = request.CardNumber.Length >= 4 ? request.CardNumber[^4..] : "****";
            return new ProcessPaymentResponseDto
            {
                IsApproved = false,
                Message = reason,
                MaskedCardNumber = $"************{last4}",
                ProcessedAt = processedAt
            };
        }
    }
}