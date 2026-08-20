using System.ComponentModel.DataAnnotations;


namespace ArtemisBankingPro.Core.Application.ViewModels.Card
{
    public class AssignCreditCardViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        [Display(Name = "Cliente")]
        public string ClientId { get; set; } = string.Empty;

        [Display(Name = "Nombre del Cliente")]
        public string ClientFullName { get; set; } = string.Empty;

        [Display(Name = "Cédula del Cliente")]
        public string Identification { get; set; } = string.Empty;

        [Required(ErrorMessage = "El límite de crédito es obligatorio.")]
        [Range(0.01, 10000000.00, ErrorMessage = "El límite de crédito debe ser mayor a RD$0.00.")]
        [Display(Name = "Límite de Crédito")]
        [DataType(DataType.Currency)]
        public decimal CreditLimit { get; set; }

        [MaxLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        [Display(Name = "Descripción u Observaciones")]
        public string? Description { get; set; }
    }
}
