using ArtemisBankingPro.Core.Application.Interfaces.Card;
using ArtemisBankingPro.Core.Application.Services;
using ArtemisBankingPro.Core.Application.Services.Card;
using ArtemisBankingPro.Core.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ArtemisBankingPro.Core.Application;

public static class ServicesRegistration
{
    public static void AddApplicationLayerIoc(this IServiceCollection services)
    {
        #region Services IOC
        services.AddScoped<IAmortizationService, CalculadoraAmortizationService>();
        services.AddScoped<GeneradorNumeroPrestamoService>();
        services.AddScoped<ICvcHashService, CvcHashService>();
        services.AddScoped<ICardNumberGeneratorService, CardNumberGeneratorService>();
        services.AddScoped<ICreditCardService, CreditCardService>();
        #endregion
       

        #region Configurations
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
        services.AddMediatR(opt => opt.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));//hay que descomentar esto cuando waldin configure
        #endregion

    }
}
