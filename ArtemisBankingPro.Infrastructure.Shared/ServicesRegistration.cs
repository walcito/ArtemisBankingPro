using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Infrastructure.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBankingPro.Infrastructure.Shared
{
    public static class ServicesRegistration
    {
        public static void AddSharedLayerIoc(this IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
