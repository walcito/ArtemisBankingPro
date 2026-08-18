using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Account;

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "El token es obligatorio.")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar la contraseña.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public bool HasError { get; set; }
    public string Error { get; set; } = string.Empty;
}