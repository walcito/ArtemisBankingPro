using ArtemisBankingPro.Core.Domain.Common;
using ArtemisBankingPro.Core.Domain.Common.Enums.Card;

namespace ArtemisBankingPro.Core.Domain.Entities.Card
{
    public class CreditCard : BaseEntity
    {
        public required int UserId { get; set; }
        public required string CardNumber { get; set; }
        public required DateTime ExpirationDate { get; set; }
        public required string CvcHash { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentDebt { get; set; }
        public decimal AvailableCredit => CreditLimit - CurrentDebt;
        public CardStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public ICollection<CardConsumption>? Consumptions { get; set; }
        public ICollection<CardProcessingLog>? ProcessingLogs { get; set; }

    }
}
