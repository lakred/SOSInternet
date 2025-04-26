using Microsoft.Extensions.DependencyInjection;
using SOSInternet.Core.Interfaces;
using SOSInternet.Core.Services;

namespace SOSInternet.Utilities.Extensions;

/// <summary>  
/// Extensions for service registration  
/// </summary>  
public static class ServiceCollectionExtensions
{
    /// <summary>  
    /// Registers the application services  
    /// </summary>  
    /// <param name="services">Service collection</param>  
    /// <returns>Configured service collection</returns>  
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    { 
        services.AddSingleton<IInternetChecker, InternetChecker>();
        services.AddSingleton<IRouterRebooter, TpLinkRouterRebooter>();
        services.AddSingleton<IConnectionMonitor, ConnectionMonitor>();
        services.AddHostedService(provider => (ConnectionMonitor)provider.GetRequiredService<IConnectionMonitor>());

        return services;
    }
}