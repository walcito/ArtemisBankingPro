using Microsoft.AspNetCore.Identity;
using ArtemisBankingPro.Infrastructure.Identity.Entities;

namespace ArtemisBankingPro.Infrastructure.Identity.Seeds;

public static class IdentitySeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var roles = new[]
        {
            new IdentityRole { Name = "Administrador", NormalizedName = "ADMINISTRADOR" },
            new IdentityRole { Name = "Cajero", NormalizedName = "CAJERO" },
            new IdentityRole { Name = "Cliente", NormalizedName = "CLIENTE" },
            new IdentityRole { Name = "Comercio", NormalizedName = "COMERCIO" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }

        var defaultAdmin = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@artemisbanking.com",
            Nombre = "Admin",
            Apellido = "Sistema",
            Cedula = "00000000000",
            EmailConfirmed = true,
            EstaActivo = true
        };

        if (await userManager.FindByEmailAsync(defaultAdmin.Email) == null)
        {
            var result = await userManager.CreateAsync(defaultAdmin, "123P@$$word!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(defaultAdmin, "Administrador");
            }
        }
    }
}
