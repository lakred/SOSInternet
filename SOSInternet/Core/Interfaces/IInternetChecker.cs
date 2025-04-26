using SOSInternet.Core.Models;

namespace SOSInternet.Core.Interfaces;

/// <summary>
/// Interface for checking internet connection
/// </summary>
public interface IInternetChecker
{
    /// <summary>
    /// Checks the internet connection
    /// </summary>
    /// <returns>A ConnectionStatus object containing the connection status and details</returns>
    Task<ConnectionStatus> CheckConnectionAsync();
}