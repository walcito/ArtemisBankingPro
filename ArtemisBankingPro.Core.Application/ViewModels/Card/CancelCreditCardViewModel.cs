using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBankingPro.Core.Application.ViewModels.Card
{
    public class CancelCreditCardViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número de Tarjeta")]
        public string MaskedCardNumber { get; set; } = string.Empty;
        public string LastFourDigits { get; set; } = string.Empty;

        [Display(Name = "Cliente")]
        public string ClientFullName { get; set; } = string.Empty;

        [Display(Name = "Monto Adeudado")]
        [DataType(DataType.Currency)]
        public decimal CurrentDebt { get; set; }

        [MaxLength(500, ErrorMessage = "El motivo no puede exceder los 500 caracteres.")]
        [Display(Name = "Motivo de Cancelación")]
        public string? Reason { get; set; }

    }
}
