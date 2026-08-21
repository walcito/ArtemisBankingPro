using ArtemisBankingPro.Core.Application.Interfaces;
using ArtemisBankingPro.Core.Application.Interfaces.Admin;
using ArtemisBankingPro.Core.Application.Interfaces.Card;
using ArtemisBankingPro.Core.Application.Interfaces.Cashier;
using ArtemisBankingPro.Core.Application.Interfaces.Client;
using ArtemisBankingPro.Core.Application.Interfaces.Loan;
using ArtemisBankingPro.Core.Application.Interfaces.SavingsAccount;
using ArtemisBankingPro.Core.Application.Services;
using ArtemisBankingPro.Core.Application.Services.Admin;
using ArtemisBankingPro.Core.Application.Services.Card;
using ArtemisBankingPro.Core.Application.Services.Cashier;
using ArtemisBankingPro.Core.Application.Services.Client;
using ArtemisBankingPro.Core.Application.Services.Loan;
using ArtemisBankingPro.Core.Application.Services.SavingsAccount;
using ArtemisBankingPro.Core.Domain.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ArtemisBankingPro.Core.Application;

public static class ServicesRegistration
{
    public static void AddApplicationLayerIoc(this IServiceCollection services)
    {
        #region Services IOC
        services.AddTransient(typeof(IGenericService<>), typeof(GenericService<>));
        services.AddScoped<IAmortizationService, AmortizationService>();
        services.AddScoped<ILoanNumberGeneratorService, LoanNumberGeneratorService>();
        services.AddScoped<ICvcHashService, CvcHashService>();
        services.AddScoped<ICardNumberGeneratorService, CardNumberGeneratorService>();
        services.AddScoped<ICreditCardService, CreditCardService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ICashierService, CashierService>();
        services.AddScoped<IClientProductService, ClientProductService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<ISavingsAccountService, SavingsAccountService>();
        #endregion
       

        #region Configurations
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
        services.AddMediatR(opt => opt.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.ValidationBehavior<,>));
        #endregion

    }
}
