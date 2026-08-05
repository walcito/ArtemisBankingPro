using ArtemisBankingPro.Core.Domain.Common.Enums.Card;

namespace ArtemisBankingPro.Core.Domain.Entities.Card
{
    public class CreditCard
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public string CvcHash { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public decimal CurrentDebt { get; set; }
        public decimal AvailableCredit => CreditLimit - CurrentDebt;
        //public CardStatus Status { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
