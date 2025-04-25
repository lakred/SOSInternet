using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SOSInternet.Core.Interfaces;
using SOSInternet.Core.Models;

namespace SOSInternet.Core.Services;

/// <summary>
/// Internet connection monitoring service
/// </summary>
public class ConnectionMonitor : BackgroundService, IConnectionMonitor
{
    private readonly ILogger<ConnectionMonitor> _logger;
    private readonly IInternetChecker _internetChecker;
    private readonly IRouterRebooter _routerRebooter;
    private readonly ConnectionSettings _settings;
    private CancellationTokenSource? _stoppingCts;
    private bool _isMonitoring;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger for activity logging</param>
    /// <param name="internetChecker">Service for connection checking</param>
    /// <param name="routerRebooter">Service for router rebooting</param>
    /// <param name="options">Configuration options</param>
    public ConnectionMonitor(
        ILogger<ConnectionMonitor> logger,
        IInternetChecker internetChecker,
        IRouterRebooter routerRebooter,
        IOptions<ConnectionSettings> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _internetChecker = internetChecker ?? throw new ArgumentNullException(nameof(internetChecker));
        _routerRebooter = routerRebooter ?? throw new ArgumentNullException(nameof(routerRebooter));
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Starts monitoring the internet connection
    /// </summary>
    /// <returns>Task representing the asynchronous monitoring operation</returns>
    public Task StartMonitoringAsync()
    {
        if (_isMonitoring)
        {
            _logger.LogWarning("Monitoring is already active");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Starting internet connection monitoring");
        _stoppingCts = new CancellationTokenSource();
        _isMonitoring = true;

        return StartAsync(_stoppingCts.Token);
    }

    /// <summary>
    /// Stops monitoring the internet connection
    /// </summary>
    public void StopMonitoring()
    {
        if (!_isMonitoring)
        {
            _logger.LogWarning("Monitoring is not active");
            return;
        }

        _logger.LogInformation("Stopping internet connection monitoring");
        _stoppingCts?.Cancel();
        _isMonitoring = false;
    }

    /// <summary>
    /// Executes the monitoring service in the background
    /// </summary>
    /// <param name="stoppingToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous monitoring operation</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Monitoring service started");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await MonitorConnectionAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(ex, "Monitoring service stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during connection monitoring: {Message}", ex.Message);
        }
        finally
        {
            _stoppingCts?.Dispose();
            _isMonitoring = false;
            _logger.LogInformation("Monitoring service terminated");
        }
    }

    /// <summary>
    /// Monitors the internet connection and reboots the router if necessary
    /// </summary>
    /// <param name="stoppingToken">Cancellation token</param>
    /// <returns>Task representing the asynchronous monitoring operation</returns>
    private async Task MonitorConnectionAsync(CancellationToken stoppingToken)
    {
        try
        {
            var status = await _internetChecker.CheckConnectionAsync();

            if (status.IsConnected)
            {
                _logger.LogInformation("Internet connection is active");
                await Task.Delay(TimeSpan.FromSeconds(_settings.CheckIntervalSeconds), stoppingToken);
                return;
            }

            _logger.LogWarning("Internet connection is not active, starting recovery attempts");

            for (int attempt = 1; attempt <= _settings.RetryAttemptsBeforeReboot; attempt++)
            {
                if (stoppingToken.IsCancellationRequested) return;

                _logger.LogWarning("Recovery attempt {Attempt}/{TotalAttempts}",
                    attempt, _settings.RetryAttemptsBeforeReboot);

                await Task.Delay(TimeSpan.FromSeconds(_settings.CheckIntervalSeconds), stoppingToken);

                status = await _internetChecker.CheckConnectionAsync();

                if (status.IsConnected)
                {
                    _logger.LogInformation("Internet connection restored on attempt {Attempt}", attempt);
                    return;
                }

                if (attempt != _settings.RetryAttemptsBeforeReboot) continue;

                _logger.LogWarning("Connection not restored after {Attempts} attempts, rebooting the router",
                    _settings.RetryAttemptsBeforeReboot);

                await _routerRebooter.RebootAsync();

                _logger.LogInformation("Router rebooted, waiting {Seconds} seconds for recovery",
                    _settings.WaitAfterRebootSeconds);

                await Task.Delay(TimeSpan.FromSeconds(_settings.WaitAfterRebootSeconds), stoppingToken);

                status = await _internetChecker.CheckConnectionAsync();

                if (status.IsConnected)
                {
                    _logger.LogInformation("Internet connection restored after router reboot");
                }
                else
                {
                    _logger.LogError("Internet connection not restored even after router reboot");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during connection monitoring: {Message}", ex.Message);
            await Task.Delay(TimeSpan.FromSeconds(_settings.CheckIntervalSeconds), stoppingToken);
        }
    }
}