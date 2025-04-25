using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SOSInternet.Core.Interfaces;
using SOSInternet.Core.Models;
using SOSInternet.Core.Services;
using SOSInternet.Infrastructure.Logging;

namespace SOSInternet.Infrastructure.Configuration
{
    /// <summary>
    /// Classe per la gestione delle configurazioni dell'applicazione
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Configura i servizi dell'applicazione
        /// </summary>
        /// <param name="services">Collezione di servizi</param>
        /// <param name="configuration">Configurazione dell'applicazione</param>
        /// <returns>Collezione di servizi configurata</returns>
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurazione del logger
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Information);
                loggingBuilder.AddNLog();
            });

            // Configurazione NLog
            LoggerConfiguration.ConfigureLogger();

            // Registrazione delle configurazioni
            services.Configure<RouterSettings>(configuration.GetSection("RouterSettings"));
            services.Configure<ConnectionSettings>(configuration.GetSection("ConnectionSettings"));

            // Registrazione dei servizi
            services.AddSingleton<IInternetChecker, InternetChecker>();
            services.AddSingleton<IRouterRebooter, TpLinkRouterRebooter>();
            services.AddSingleton<IConnectionMonitor, ConnectionMonitor>();
            services.AddHostedService(provider => (ConnectionMonitor)provider.GetRequiredService<IConnectionMonitor>());

            return services;
        }

        /// <summary>
        /// Crea la configurazione dell'applicazione
        /// </summary>
        /// <param name="args">Argomenti della riga di comando</param>
        /// <returns>Configurazione dell'applicazione</returns>
        public static IConfiguration CreateConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
        }
    }
}
