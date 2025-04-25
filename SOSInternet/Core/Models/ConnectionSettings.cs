namespace SOSInternet.Core.Models
{
    /// <summary>
    /// Modello per le impostazioni di connessione
    /// </summary>
    public class ConnectionSettings
    {
        /// <summary>
        /// Intervallo in secondi tra i controlli della connessione
        /// </summary>
        public int CheckIntervalSeconds { get; set; } = 15;

        /// <summary>
        /// Numero di tentativi prima di riavviare il router
        /// </summary>
        public int RetryAttemptsBeforeReboot { get; set; } = 4;

        /// <summary>
        /// Tempo di attesa in secondi dopo il riavvio del router
        /// </summary>
        public int WaitAfterRebootSeconds { get; set; } = 120;

        /// <summary>
        /// Elenco di indirizzi da pingare per verificare la connessione
        /// </summary>
        public List<string> PingTargets { get; set; } = new List<string> { "8.8.8.8", "1.1.1.1", "google.com" };
    }
}
