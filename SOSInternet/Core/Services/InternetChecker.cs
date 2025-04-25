using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SOSInternet.Core.Interfaces;
using SOSInternet.Core.Models;
using System.Net.NetworkInformation;
using System.Text;

namespace SOSInternet.Core.Services
{
    /// <summary>
    /// Implementazione del controllo della connessione internet
    /// </summary>
    public class InternetChecker : IInternetChecker
    {
        private readonly ILogger<InternetChecker> _logger;
        private readonly ConnectionSettings _settings;

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="logger">Logger per la registrazione delle attività</param>
        /// <param name="options">Opzioni di configurazione</param>
        public InternetChecker(ILogger<InternetChecker> logger, IOptions<ConnectionSettings> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Verifica la connessione internet
        /// </summary>
        /// <returns>Un oggetto ConnectionStatus contenente lo stato della connessione e i dettagli</returns>
        public async Task<ConnectionStatus> CheckConnectionAsync()
        {
            _logger.LogDebug("Inizio verifica della connessione internet");
            
            var sb = new StringBuilder();
            bool isConnected = false;
            long? responseTime = null;

            try
            {
                // Verifica la connessione pingando più target
                foreach (var target in _settings.PingTargets)
                {
                    try
                    {
                        var pingResult = await PingHostAsync(target);
                        sb.AppendLine($"Ping a {target}: {(pingResult.Success ? "Riuscito" : "Fallito")}");
                        
                        if (pingResult.Success)
                        {
                            sb.AppendLine($"  Tempo di risposta: {pingResult.RoundtripTime} ms");
                            isConnected = true;
                            responseTime = pingResult.RoundtripTime;
                            break; // Se almeno un ping ha successo, la connessione è attiva
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Errore durante il ping a {target}: {ex.Message}");
                    }
                }

                // Verifica lo stato delle interfacce di rete
                sb.AppendLine("\nStato delle interfacce di rete:");
                foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || 
                        networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        sb.AppendLine($"  {networkInterface.Name}: {networkInterface.OperationalStatus}");
                    }
                }

                var status = new ConnectionStatus(isConnected, sb.ToString())
                {
                    ResponseTime = responseTime
                };

                _logger.LogInformation("Verifica connessione completata: {IsConnected}", isConnected);
                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica della connessione");
                return ConnectionStatus.CreateError(ex.Message);
            }
        }

        private async Task<(bool Success, long RoundtripTime)> PingHostAsync(string host)
        {
            using var ping = new Ping();
            try
            {
                var reply = await ping.SendPingAsync(host, 3000); // 3 secondi di timeout
                return (reply.Status == IPStatus.Success, reply.RoundtripTime);
            }
            catch
            {
                return (false, 0);
            }
        }
    }
}
