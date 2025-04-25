namespace SOSInternet.Core.Models;

/// <summary>
/// Model for connection settings
/// </summary>
public class ConnectionSettings
{
    /// <summary>
    /// Interval in seconds between connection checks
    /// </summary>
    public int CheckIntervalSeconds { get; set; } = 15;

    /// <summary>
    /// Number of attempts before rebooting the router
    /// </summary>
    public int RetryAttemptsBeforeReboot { get; set; } = 4;

    /// <summary>
    /// Waiting time in seconds after rebooting the router
    /// </summary>
    public int WaitAfterRebootSeconds { get; set; } = 120;

    /// <summary>
    /// List of addresses to ping to verify the connection
    /// </summary>
    public List<string> PingTargets { get; set; } = ["8.8.8.8", "1.1.1.1", "google.com"];
}