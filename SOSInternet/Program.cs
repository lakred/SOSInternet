using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SOSInternet.Core.Interfaces;
using SOSInternet.Infrastructure.Configuration;
using SOSInternet.Utilities.Playwright;

namespace SOSInternet
{ 
    public static class Program
    { 
        public static async Task Main(string[] args)
        {
            // Crea l'host dell'applicazione
            using var host = CreateHostBuilder(args).Build();

            // Ottieni il servizio di monitoraggio
            var monitor = host.Services.GetRequiredService<IConnectionMonitor>();

            await PlaywrightSetup.EnsureBrowsersInstalledAsync();
            // Avvia l'host
            await host.StartAsync();

            // Avvia il monitoraggio
            await monitor.StartMonitoringAsync();

            // Attendi la chiusura dell'applicazione
            await host.WaitForShutdownAsync();
        }

        /// <summary>
        /// Crea il builder dell'host dell'applicazione
        /// </summary>
        /// <param name="args">Argomenti della riga di comando</param>
        /// <returns>Builder dell'host configurato</returns>
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Configura i servizi dell'applicazione
                    services.ConfigureServices(AppSettings.CreateConfiguration(args));
                });
    }
}