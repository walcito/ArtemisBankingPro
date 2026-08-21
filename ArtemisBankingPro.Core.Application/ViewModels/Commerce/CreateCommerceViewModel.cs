using System.ComponentModel.DataAnnotations;

namespace ArtemisBankingPro.Core.Application.ViewModels.Commerce;

public class CreateCommerceViewModel
{
    [Required(ErrorMessage = "El nombre del comercio es obligatorio.")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]
    public string Mail { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "El RNC es obligatorio.")]
    public string Rnc { get; set; } = string.Empty;
}
