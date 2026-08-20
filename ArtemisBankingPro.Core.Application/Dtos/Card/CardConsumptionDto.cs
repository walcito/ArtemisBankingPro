using ArtemisBankingPro.Core.Domain.Common.Enums.Card;


namespace ArtemisBankingPro.Core.Application.Dtos.Card
{
    public class CardConsumptionDto
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string MaskedCardNumber { get; set; } = string.Empty;
        public int CommerceId { get; set; }
        public string CommerceName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public ConsumptionStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsApproved => Status == ConsumptionStatus.Approved;
    }
}
