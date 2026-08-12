using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using System.ComponentModel.DataAnnotations;


namespace ArtemisBankingPro.Core.Application.ViewModels.Card
{
    public class CreditCardViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [Display(Name = "Cliente")]
        public string ClientFullName { get; set; } = string.Empty;

        [Display(Name = "Número de Tarjeta")]
        public string MaskedCardNumber { get; set; } = string.Empty;
        public string LastFourDigits { get; set; } = string.Empty;

        [Display(Name = "Límite de Crédito")]
        [DataType(DataType.Currency)]
        public decimal CreditLimit { get; set; }

        [Display(Name = "Crédito Disponible")]
        [DataType(DataType.Currency)]
        public decimal AvailableCredit { get; set; }

        [Display(Name = "Monto Adeudado")]
        [DataType(DataType.Currency)]
        public decimal CurrentDebt { get; set; }

        [Display(Name = "Fecha de Expiración")]
        public string ExpirationDate { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public CardStatus Status { get; set; }
        public bool IsActive => Status == CardStatus.Active;

        [Display(Name = "Fecha de Asignación")]
        public DateTime CreatedAt { get; set; }
    }
}
