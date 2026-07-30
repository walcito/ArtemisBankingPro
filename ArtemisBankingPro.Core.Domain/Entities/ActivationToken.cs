using ArtemisBankingPro.Core.Domain.Common;

namespace ArtemisBankingPro.Core.Domain.Entities;

public class ActivationToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public string UsuarioId { get; set; } = string.Empty;
    public bool Usado { get; set; } = false;
    public DateTime FechaExpiracion { get; set; }
}
