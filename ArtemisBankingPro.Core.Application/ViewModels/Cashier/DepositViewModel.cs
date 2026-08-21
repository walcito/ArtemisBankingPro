using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Cashier;

public class DepositViewModel
{
    [Required(ErrorMessage = "El número de cuenta es obligatorio.")]
    public string AccountNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
    public decimal Amount { get; set; }
}
