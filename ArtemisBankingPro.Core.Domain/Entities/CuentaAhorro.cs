using ArtemisBankingPro.Core.Domain.Common.Enums;

namespace ArtemisBankingPro.Core.Domain.Entities;

public class CuentaAhorro
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0.00m;
    public TipoCuenta Tipo { get; set; } = TipoCuenta.Secundaria;
    public EstadoCuenta Estado { get; set; } = EstadoCuenta.Activa;
    public int UsuarioId { get; set; }
    public User? Usuario { get; set; }
}
