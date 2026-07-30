using Microsoft.AspNetCore.Identity;

namespace ArtemisBankingPro.Infrastructure.Identity.Entities;

public class ApplicationUser : IdentityUser
{
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
    public required string Cedula { get; set; }
    public bool EstaActivo { get; set; } = true;
}
