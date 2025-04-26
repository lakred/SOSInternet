namespace SOSInternet.Core.Interfaces;

/// <summary>  
/// Interface for monitoring the internet connection.  
/// </summary>  
public interface IConnectionMonitor
{
    /// <summary>  
    /// Starts monitoring the internet connection asynchronously.  
    /// </summary>
    Task StartMonitoringAsync();

    /// <summary>  
    /// Stops monitoring the internet connection.  
    /// </summary>  
    void StopMonitoring();
}