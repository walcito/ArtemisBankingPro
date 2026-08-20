using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Account;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    public string UserName { get; set; } = string.Empty;

    public bool HasError { get; set; }
    public string Error { get; set; } = string.Empty;
}