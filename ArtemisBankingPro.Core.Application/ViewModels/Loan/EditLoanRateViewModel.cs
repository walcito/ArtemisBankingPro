using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Loan;

public class EditLoanRateViewModel
{
    public int LoanId { get; set; }
    public string LoanNumber { get; set; } = string.Empty;
    public decimal CurrentRate { get; set; }

    [Required(ErrorMessage = "La nueva tasa es requerida.")]
    [Range(0.00, double.MaxValue, ErrorMessage = "La tasa de interés anual no puede ser negativa.")]
    public decimal NewAnnualInterestRate { get; set; }
}
