using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Loan;

public class AssignLoanFormViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un cliente.")]
    public int UserId { get; set; }

    public string ClientFullName { get; set; } = string.Empty;
    public string ClientIdentification { get; set; } = string.Empty;

    [Required(ErrorMessage = "El plazo del préstamo es requerido.")]
    public int TermInMonths { get; set; }

    [Required(ErrorMessage = "El monto a prestar es requerido.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto a prestar debe ser mayor que cero.")]
    public decimal DisbursedAmount { get; set; }

    [Required(ErrorMessage = "La tasa de interés anual es requerida.")]
    [Range(0.00, double.MaxValue, ErrorMessage = "La tasa de interés anual no puede ser negativa.")]
    public decimal AnnualInterestRate { get; set; }
}
