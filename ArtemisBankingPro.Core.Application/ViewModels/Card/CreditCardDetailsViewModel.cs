using ArtemisBankingPro.Core.Domain.Common.Enums.Card;
using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Card
{
    public class CreditCardDetailsViewModel
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

        [Display(Name = "Fecha de Asignación")]
        public DateTime CreatedAt { get; set; }
        public List<CardConsumptionViewModel> Consumptions { get; set; } = new();
    }
    public class CardConsumptionViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Fecha del Consumo")]
        public DateTime Date { get; set; }

        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Comercio")]
        public string CommerceName { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public ConsumptionStatus Status { get; set; }
        public bool IsApproved => Status == ConsumptionStatus.Approved;
    }
}
