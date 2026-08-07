using ArtemisBankingPro.Core.Application.Services;
using ArtemisBankingPro.Core.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBankingPro.Core.Application;

public static class ServicesRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAmortizationService, CalculadoraAmortizationService>();

        return services;
    }
}
