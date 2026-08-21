using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Cashier;

public class ThirdPartyTransactionViewModel
{
    [Required(ErrorMessage = "La cuenta de origen es obligatoria.")]
    public string SourceAccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cuenta de destino es obligatoria.")]
    public string DestinationAccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
    public decimal Amount { get; set; }
}
