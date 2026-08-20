using ArtemisBankingPro.Core.Domain.Common;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;

namespace ArtemisBankingPro.Core.Domain.Entities.Card
{
    public class CardConsumption : BaseEntity
    {
        public required int CardId { get; set; }
        public required int CommerceId { get; set; }
        public decimal Amount { get; set; }
        public ConsumptionStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? RejectionReason { get; set; }
        public int? ProcessingLogId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public CreditCard? CreditCard { get; set; }
        public CardProcessingLog? ProcessingLog { get; set; }
  
    }
}
