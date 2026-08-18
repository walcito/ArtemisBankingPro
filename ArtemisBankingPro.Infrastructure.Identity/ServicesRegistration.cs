using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Infrastructure.Identity.Contexts;
using ArtemisBankingPro.Infrastructure.Identity.Entities;
using ArtemisBankingPro.Infrastructure.Identity.Seeds;
using ArtemisBankingPro.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBankingPro.Infrastructure.Identity;

public static class ServicesRegistration
{
    public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<IdentityContext>(options =>
            options.UseSqlServer(config.GetConnectionString("IdentityConnection"),
            b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        #region Services
        //services.AddTransient<IAccountService, AccountService>();
        //services.AddTransient<IUserService, UserService>();
        #endregion
    }

    public static async Task RunIdentitySeedAsync(this IServiceProvider service)
    {
        using var scope = service.CreateScope();
        var servicesProvider = scope.ServiceProvider;

        var userManager = servicesProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = servicesProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await IdentitySeeder.SeedAsync(userManager, roleManager);
    }
}