namespace SOSInternet.Core.Models;

/// <summary>
/// Model for the internet connection status
/// </summary>
public class ConnectionStatus
{
    /// <summary>
    /// Indicates whether the internet connection is active
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Details about the internet connection
    /// </summary>
    public string Details { get; set; }

    /// <summary>
    /// Timestamp of the check
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Response time in milliseconds (if available)
    /// </summary>
    public long? ResponseTime { get; set; }

    /// <summary>
    /// Any errors encountered during the check
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Creates a new instance of ConnectionStatus
    /// </summary>
    /// <param name="isConnected">Indicates whether the connection is active</param>
    /// <param name="details">Details about the connection</param>
    public ConnectionStatus(bool isConnected, string details)
    {
        IsConnected = isConnected;
        Details = details;
    }

    /// <summary>
    /// Creates a new instance of ConnectionStatus for an error
    /// </summary>
    /// <param name="error">Error message</param>
    /// <returns>A ConnectionStatus object representing an error</returns>
    public static ConnectionStatus CreateError(string error)
    {
        return new ConnectionStatus(false, "Error during connection check")
        {
            Error = error
        };
    }
}