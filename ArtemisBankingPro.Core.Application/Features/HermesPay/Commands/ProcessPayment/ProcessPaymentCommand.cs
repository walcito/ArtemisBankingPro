using MediatR;

namespace ArtemisBankingPro.Core.Application.Features.HermesPay.Commands.ProcessPayment
{
    public class ProcessPaymentCommand : IRequest<ProcessPaymentResponseDto>
    {
        public int CommerceId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string MonthExpirationCard { get; set; } = string.Empty;
        public string YearExpirationCard { get; set; } = string.Empty;
        public string Cvc { get; set; } = string.Empty;
        public decimal TransactionAmount { get; set; }
        public string Description { get; set; } = "Consumo Hermes Pay";
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
    public class ProcessPaymentResponseDto
    {
        public bool IsApproved { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? ConsumptionId { get; set; }
        public string MaskedCardNumber { get; set; } = string.Empty;
        public decimal? ApprovedAmount { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
