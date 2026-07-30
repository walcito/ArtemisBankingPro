namespace ArtemisBankingPro.Core.Domain.Entities;

public class Comercio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Rnc { get; set; } = string.Empty;
    public bool EstaActivo { get; set; } = true;
    public User? UsuarioAsociado { get; set; }
}
