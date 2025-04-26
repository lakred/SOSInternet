namespace SOSInternet.Core.Models
{
    /// <summary>
    /// Model for router settings
    /// </summary>
    public class RouterSettings
    {
        /// <summary>
        /// Router administration URL
        /// </summary>
        public string Url { get; set; } = "http://192.168.1.1";

        /// <summary>
        /// Username for router access (if required)
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Password for router access
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the password is encrypted
        /// </summary>
        public bool UseEncryption { get; set; } = false;
    }
}
