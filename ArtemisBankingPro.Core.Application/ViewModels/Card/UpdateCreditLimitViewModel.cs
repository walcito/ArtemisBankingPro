using System.ComponentModel.DataAnnotations;


namespace ArtemisBankingPro.Core.Application.ViewModels.Card
{
    public class UpdateCreditLimitViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número de Tarjeta")]
        public string MaskedCardNumber { get; set; } = string.Empty;

        [Display(Name = "Cliente")]
        public string ClientFullName { get; set; } = string.Empty;

        [Display(Name = "Límite de Crédito Actual")]
        [DataType(DataType.Currency)]
        public decimal CurrentCreditLimit { get; set; }

        [Display(Name = "Deuda Actual")]
        [DataType(DataType.Currency)]
        public decimal CurrentDebt { get; set; }

        [Required(ErrorMessage = "El nuevo límite de crédito es obligatorio.")]
        [Range(0.01, 10000000.00, ErrorMessage = "El nuevo límite de crédito debe ser mayor a RD$0.00.")]
        [Display(Name = "Nuevo Límite de Crédito")]
        [DataType(DataType.Currency)]
        public decimal NewCreditLimit { get; set; }
    }
}
