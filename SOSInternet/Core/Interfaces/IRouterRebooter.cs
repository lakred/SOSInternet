namespace SOSInternet.Core.Interfaces;

/// <summary>
/// Interface for router rebooting
/// </summary>
public interface IRouterRebooter
{
    /// <summary>
    /// Reboots the router
    /// </summary>
    /// <returns>A Task representing the asynchronous reboot operation</returns>
    Task RebootAsync();
}