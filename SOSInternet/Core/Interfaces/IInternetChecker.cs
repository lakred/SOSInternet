using SOSInternet.Core.Models;

namespace SOSInternet.Core.Interfaces
{
    /// <summary>
    /// Interfaccia per il controllo della connessione internet
    /// </summary>
    public interface IInternetChecker
    {
        /// <summary>
        /// Verifica la connessione internet
        /// </summary>
        /// <returns>Un oggetto ConnectionStatus contenente lo stato della connessione e i dettagli</returns>
        Task<ConnectionStatus> CheckConnectionAsync();
    }
}
