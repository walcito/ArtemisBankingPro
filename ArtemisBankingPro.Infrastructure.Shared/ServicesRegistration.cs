using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Domain.Settings;
using ArtemisBankingPro.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBankingPro.Infrastructure.Shared;

public static class ServicesRegistration
{
    public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        #region Configurations
        services.Configure<MailSettings>(config.GetSection("MailSettings"));
        #endregion

        #region Services IOC
        services.AddTransient<IEmailService, EmailService>();
        #endregion
    }
}