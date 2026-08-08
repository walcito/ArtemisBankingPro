using Microsoft.AspNetCore.Identity;
using ArtemisBankingPro.Infrastructure.Identity.Entities;

namespace ArtemisBankingPro.Infrastructure.Identity.Seeds;

public static class IdentitySeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var roles = new[]
        {
            new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Name = "Teller", NormalizedName = "TELLER" },
            new IdentityRole { Name = "Client", NormalizedName = "CLIENT" },
            new IdentityRole { Name = "Merchant", NormalizedName = "MERCHANT" }
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
            FirstName = "Admin",
            LastName = "System",
            IdentificationNumber = "00000000000",
            EmailConfirmed = true,
            IsActive = true
        };

        if (await userManager.FindByEmailAsync(defaultAdmin.Email) == null)
        {
            var result = await userManager.CreateAsync(defaultAdmin, "123P@$$word!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(defaultAdmin, "Admin");
            }
        }
    }
}