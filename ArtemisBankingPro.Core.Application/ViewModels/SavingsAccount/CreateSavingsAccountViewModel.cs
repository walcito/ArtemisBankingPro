using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.SavingsAccount;

public class CreateSavingsAccountViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    public int UserId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El monto inicial no puede ser negativo.")]
    public decimal InitialAmount { get; set; } = 0.00m;
    public int DomainUserId { get; set; }
}
