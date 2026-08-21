using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Client;

public class TransferToBeneficiaryViewModel
{
    [Required(ErrorMessage = "La cuenta de origen es obligatoria.")]
    public string SourceAccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un beneficiario.")]
    public int BeneficiaryId { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
    public decimal Amount { get; set; }
}
