namespace SOSInternet.Core.Models
{
    /// <summary>
    /// Modello per lo stato della connessione internet
    /// </summary>
    public class ConnectionStatus
    {
        /// <summary>
        /// Indica se la connessione internet è attiva
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Dettagli sulla connessione internet
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Timestamp della verifica
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Tempo di risposta in millisecondi (se disponibile)
        /// </summary>
        public long? ResponseTime { get; set; }

        /// <summary>
        /// Eventuali errori riscontrati durante la verifica
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Crea una nuova istanza di ConnectionStatus
        /// </summary>
        /// <param name="isConnected">Indica se la connessione è attiva</param>
        /// <param name="details">Dettagli sulla connessione</param>
        public ConnectionStatus(bool isConnected, string details)
        {
            IsConnected = isConnected;
            Details = details;
        }

        /// <summary>
        /// Crea una nuova istanza di ConnectionStatus per un errore
        /// </summary>
        /// <param name="error">Messaggio di errore</param>
        /// <returns>Un oggetto ConnectionStatus che rappresenta un errore</returns>
        public static ConnectionStatus CreateError(string error)
        {
            return new ConnectionStatus(false, "Errore durante la verifica della connessione")
            {
                Error = error
            };
        }
    }
}
