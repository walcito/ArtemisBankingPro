using Microsoft.AspNetCore.Identity;

namespace ArtemisBankingPro.Infrastructure.Identity.Entities;

public class ApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string IdentificationNumber { get; set; }
    public bool IsActive { get; set; } = true;
}