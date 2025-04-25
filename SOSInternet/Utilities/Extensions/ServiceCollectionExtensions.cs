using Microsoft.Extensions.DependencyInjection;
using SOSInternet.Core.Interfaces;
using SOSInternet.Core.Services;

namespace SOSInternet.Utilities.Extensions
{
    /// <summary>
    /// Estensioni per la registrazione dei servizi
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra i servizi dell'applicazione
        /// </summary>
        /// <param name="services">Collezione di servizi</param>
        /// <returns>Collezione di servizi configurata</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrazione dei servizi
            services.AddSingleton<IInternetChecker, InternetChecker>();
            services.AddSingleton<IRouterRebooter, TpLinkRouterRebooter>();
            services.AddSingleton<IConnectionMonitor, ConnectionMonitor>();
            services.AddHostedService(provider => (ConnectionMonitor)provider.GetRequiredService<IConnectionMonitor>());

            return services;
        }
    }
}
