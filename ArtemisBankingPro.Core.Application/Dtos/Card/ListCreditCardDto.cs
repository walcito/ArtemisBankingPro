using ArtemisBankingPro.Core.Domain.Common.Enums.Card;

namespace ArtemisBankingPro.Core.Application.Dtos.Card
{
    public class ListCreditCardDto
    {
        public int Id { get; set; }
        public string MaskedCardNumber { get; set; } = string.Empty;
        public string LastFourDigits { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string ClientFullName { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public decimal AvailableCredit { get; set; }
        public decimal CurrentDebt { get; set; }
        public string ExpirationDate { get; set; } = string.Empty;
        public CardStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
