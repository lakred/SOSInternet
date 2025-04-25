using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using SOSInternet.Core.Interfaces;
using SOSInternet.Core.Models;
using System.Security.Cryptography;
using System.Text;

namespace SOSInternet.Core.Services
{
    /// <summary>
    /// Implementazione del riavvio del router TP-Link
    /// </summary>
    public class TpLinkRouterRebooter : IRouterRebooter
    {
        private readonly ILogger<TpLinkRouterRebooter> _logger;
        private readonly RouterSettings _settings;

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="logger">Logger per la registrazione delle attività</param>
        /// <param name="options">Opzioni di configurazione</param>
        public TpLinkRouterRebooter(ILogger<TpLinkRouterRebooter> logger, IOptions<RouterSettings> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Riavvia il router
        /// </summary>
        /// <returns>Task che rappresenta l'operazione asincrona di riavvio</returns>
        public async Task RebootAsync()
        {
            _logger.LogInformation("Avvio procedura di riavvio del router TP-Link");

            try
            {
                // Ottieni la password decriptata se necessario
                string password = _settings.UseEncryption 
                    ? DecryptPassword(_settings.Password) 
                    : _settings.Password;

                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true // Esecuzione browser in background
                });

                var page = await browser.NewPageAsync();
                _logger.LogDebug("Browser avviato con successo");

                // Naviga alla pagina di login del router
                await page.GotoAsync(_settings.Url);
                _logger.LogInformation("Navigazione alla pagina di login del router: {Url}", _settings.Url);
                
                // Attendi il caricamento della pagina
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                // Effettua il login
                await page.FillAsync("input#pc-login-password", password);
                _logger.LogDebug("Password inserita");
                
                await page.ClickAsync("#pc-login-btn");
                _logger.LogInformation("Login effettuato");
                
                // Attendi il caricamento della pagina dopo il login
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Clicca sul pulsante di riavvio
                await page.ClickAsync("a#topReboot");
                await page.ClickAsync(".button-buttonlarge.green.pure-button.btn-msg.btn-msg-ok.btn-confirm");
                _logger.LogInformation("Richiesta di riavvio inviata");

                // Attendi la finestra di dialogo di conferma e accettala
                await page.WaitForSelectorAsync("div.dialog-btn-container");
                await page.ClickAsync("div.dialog-btn-container button.T_ok");
                _logger.LogInformation("Riavvio confermato");
                
                _logger.LogInformation("Procedura di riavvio completata con successo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il riavvio del router: {Message}", ex.Message);
                throw new ApplicationException("Errore durante il riavvio del router", ex);
            }
        }

        /// <summary>
        /// Decripta la password utilizzando un algoritmo sicuro
        /// </summary>
        /// <param name="encryptedPassword">Password criptata</param>
        /// <returns>Password decriptata</returns>
        private string DecryptPassword(string encryptedPassword)
        {
            try
            {
                // Nota: In un'implementazione reale, utilizzare un sistema di gestione delle chiavi più sicuro
                // Questo è solo un esempio semplificato
                string key = Environment.GetEnvironmentVariable("SOSINTERNET_ENCRYPTION_KEY") ?? "SosInternetDefaultKey";
                
                // Implementazione semplificata per esempio
                // In produzione, utilizzare un sistema più robusto con IV e salt
                using var aes = Aes.Create();
                var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.Key = keyBytes;
                aes.IV = new byte[16]; // IV fisso per semplicità, in produzione usare un IV casuale e salvarlo
                
                var encryptedBytes = Convert.FromBase64String(encryptedPassword);
                using var decryptor = aes.CreateDecryptor();
                using var ms = new MemoryStream(encryptedBytes);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cs);
                
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la decriptazione della password");
                throw new ApplicationException("Impossibile decriptare la password", ex);
            }
        }
    }
}
