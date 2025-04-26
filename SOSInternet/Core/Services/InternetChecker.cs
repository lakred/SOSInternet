using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SOSInternet.Core.Interfaces;
using SOSInternet.Core.Models;
using System.Net.NetworkInformation;
using System.Text;

namespace SOSInternet.Core.Services;

/// <summary>
/// Implementation of internet connection checking
/// </summary>
public class InternetChecker : IInternetChecker
{
    private readonly ILogger<InternetChecker> _logger;
    private readonly ConnectionSettings _settings;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger for activity logging</param>
    /// <param name="options">Configuration options</param>
    public InternetChecker(ILogger<InternetChecker> logger, IOptions<ConnectionSettings> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Checks the internet connection
    /// </summary>
    /// <returns>A ConnectionStatus object containing the connection status and details</returns>
    public async Task<ConnectionStatus> CheckConnectionAsync()
    {
        _logger.LogDebug("Starting internet connection check");

        var sb = new StringBuilder();
        var isConnected = false;
        long? responseTime = null;

        try
        {
            foreach (var target in _settings.PingTargets)
            {
                try
                {
                    var pingResult = await PingHostAsync(target);
                    sb.AppendLine($"Ping to {target}: {(pingResult.Success ? "Succeeded" : "Failed")}");

                    if (!pingResult.Success) continue;

                    sb.AppendLine($"  Response time: {pingResult.RoundtripTime} ms");
                    isConnected = true;
                    responseTime = pingResult.RoundtripTime;
                    break; // If at least one ping succeeds, the connection is active
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"Error during ping to {target}: {ex.Message}");
                }
            }

            sb.AppendLine("\nNetwork interface status:");
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

            _logger.LogInformation("Connection check completed: {IsConnected}", isConnected);
            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during connection check");
            return ConnectionStatus.CreateError(ex.Message);
        }
    }

    /// <summary>
    /// Pings a host and returns the result
    /// </summary>
    /// <param name="host">The host to ping</param>
    /// <returns>A tuple containing success status and roundtrip time</returns>
    private static async Task<(bool Success, long RoundtripTime)> PingHostAsync(string host)
    {
        using var ping = new Ping();
        try
        {
            var reply = await ping.SendPingAsync(host, 3000); 
            return (reply.Status == IPStatus.Success, reply.RoundtripTime);
        }
        catch
        {
            return (false, 0);
        }
    }
}