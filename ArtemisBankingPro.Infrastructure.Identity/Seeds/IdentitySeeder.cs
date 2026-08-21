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
            new IdentityRole { Name = "Administrador", NormalizedName = "ADMINISTRADOR" },
            new IdentityRole { Name = "Teller", NormalizedName = "TELLER" },
            new IdentityRole { Name = "Cashier", NormalizedName = "CASHIER" },
            new IdentityRole { Name = "Cajero", NormalizedName = "CAJERO" },
            new IdentityRole { Name = "Client", NormalizedName = "CLIENT" },
            new IdentityRole { Name = "Cliente", NormalizedName = "CLIENTE" },
            new IdentityRole { Name = "Merchant", NormalizedName = "MERCHANT" },
            new IdentityRole { Name = "Commerce", NormalizedName = "COMMERCE" },
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
                await userManager.AddToRoleAsync(defaultAdmin, "Administrador");
            }
        }

        var defaultCashier = new ApplicationUser
        {
            UserName = "cajero",
            Email = "cajero@artemisbanking.com",
            FirstName = "Cajero",
            LastName = "Ventanilla",
            IdentificationNumber = "00000000001",
            EmailConfirmed = true,
            IsActive = true
        };

        if (await userManager.FindByEmailAsync(defaultCashier.Email) == null)
        {
            var result = await userManager.CreateAsync(defaultCashier, "123P@$$word!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(defaultCashier, "Cashier");
                await userManager.AddToRoleAsync(defaultCashier, "Cajero");
                await userManager.AddToRoleAsync(defaultCashier, "Teller");
            }
        }

        var defaultClients = new[]
        {
            new ApplicationUser
            {
                UserName = "juanp",
                Email = "juan.perez@example.com",
                FirstName = "Juan",
                LastName = "Pérez",
                IdentificationNumber = "40200000001",
                EmailConfirmed = true,
                IsActive = true
            },
            new ApplicationUser
            {
                UserName = "mariar",
                Email = "maria.rodriguez@example.com",
                FirstName = "María",
                LastName = "Rodríguez",
                IdentificationNumber = "40200000002",
                EmailConfirmed = true,
                IsActive = true
            },
            new ApplicationUser
            {
                UserName = "pedrom",
                Email = "pedro.martinez@example.com",
                FirstName = "Pedro",
                LastName = "Martínez",
                IdentificationNumber = "40200000003",
                EmailConfirmed = true,
                IsActive = true
            }
        };

        foreach (var client in defaultClients)
        {
            if (await userManager.FindByEmailAsync(client.Email!) == null)
            {
                var result = await userManager.CreateAsync(client, "123P@$$word!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(client, "Client");
                    await userManager.AddToRoleAsync(client, "Cliente");
                }
            }
        }
    }
}