using ArtemisBankingPro.Core.Domain.Common.Enums;
using ArtemisBankingPro.Core.Domain.Entities.Card;

namespace ArtemisBankingPro.Core.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public Roles Rol { get; set; }
    public bool EstaActivo { get; set; } = false;
    public int? ComercioId { get; set; }
    public Comercio? Comercio { get; set; }
    public ICollection<CuentaAhorro> CuentasAhorro { get; set; } = new List<CuentaAhorro>();
    public ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>();
}
