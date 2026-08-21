using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Client;

public class CashAdvanceViewModel
{
    [Required(ErrorMessage = "Debe seleccionar una tarjeta de crédito.")]
    public int CreditCardId { get; set; }

    [Required(ErrorMessage = "La cuenta de destino es obligatoria.")]
    public string DestinationAccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
    public decimal Amount { get; set; }
}
