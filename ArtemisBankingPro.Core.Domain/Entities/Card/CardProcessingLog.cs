using ArtemisBankingPro.Core.Domain.Common;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;

namespace ArtemisBankingPro.Core.Domain.Entities.Card
{
    public class CardProcessingLog : BaseEntity
    {
        public required int CardId { get; set; }
        public required int CommerceId { get; set; }
        public decimal RequestedAmount { get; set; }
        public ProcessingStatus ProcessingStatus { get; set; }
        public string? ValidationErrors { get; set; }
        public DateTime ProcessedAt { get; set; }
        public int? ApprovedConsumptionId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public CreditCard? CreditCard { get; set; }
        public CardConsumption? ApprovedConsumption { get; set; }
    }
}
