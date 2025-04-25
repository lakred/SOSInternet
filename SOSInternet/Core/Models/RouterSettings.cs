namespace SOSInternet.Core.Models
{
    /// <summary>
    /// Modello per le impostazioni del router
    /// </summary>
    public class RouterSettings
    {
        /// <summary>
        /// URL di amministrazione del router
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Nome utente per l'accesso al router (se necessario)
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Password per l'accesso al router
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Indica se la password è criptata
        /// </summary>
        public bool UseEncryption { get; set; } = false;
    }
}
