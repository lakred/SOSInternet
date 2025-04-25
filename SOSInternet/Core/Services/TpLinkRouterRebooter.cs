using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using SOSInternet.Core.Interfaces;
using SOSInternet.Core.Models;
using System.Security.Cryptography;
using System.Text;

namespace SOSInternet.Core.Services;

/// <summary>
/// Implementation of TP-Link router reboot functionality
/// </summary>
public class TpLinkRouterRebooter : IRouterRebooter
{
    private readonly ILogger<TpLinkRouterRebooter> _logger;
    private readonly RouterSettings _settings;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger for activity logging</param>
    /// <param name="options">Configuration options</param>
    public TpLinkRouterRebooter(ILogger<TpLinkRouterRebooter> logger, IOptions<RouterSettings> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Reboots the router
    /// </summary>
    /// <returns>A Task representing the asynchronous reboot operation</returns>
    public async Task RebootAsync()
    {
        _logger.LogInformation("Starting TP-Link router reboot procedure");

        try
        {
            // Get the decrypted password if necessary
            string password = _settings.UseEncryption
                ? DecryptPassword(_settings.Password)
                : _settings.Password;

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true // Run browser in background
            });

            var page = await browser.NewPageAsync();
            _logger.LogDebug("Browser successfully started");

            // Navigate to the router login page
            await page.GotoAsync(_settings.Url);
            _logger.LogInformation("Navigating to the router login page: {Url}", _settings.Url);

            // Wait for the page to load
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Perform login
            await page.FillAsync("input#pc-login-password", password);
            _logger.LogDebug("Password entered");

            await page.ClickAsync("#pc-login-btn");
            _logger.LogInformation("Login successful");

            // Wait for the page to load after login
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Click the reboot button
            await page.ClickAsync("a#topReboot");
            await page.ClickAsync(".button-buttonlarge.green.pure-button.btn-msg.btn-msg-ok.btn-confirm");
            _logger.LogInformation("Reboot request sent");

            // Wait for the confirmation dialog and accept it
            await page.WaitForSelectorAsync("div.dialog-btn-container");
            await page.ClickAsync("div.dialog-btn-container button.T_ok");
            _logger.LogInformation("Reboot confirmed");

            _logger.LogInformation("Reboot procedure completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during router reboot: {Message}", ex.Message);
            throw new InvalidOperationException("Error during router reboot", ex);
        }
    }

    /// <summary>
    /// Decrypts the password using a secure algorithm
    /// </summary>
    /// <param name="encryptedPassword">Encrypted password</param>
    /// <returns>Decrypted password</returns>
    private string DecryptPassword(string encryptedPassword)
    {
        try
        {
            // Note: In a real implementation, use a more secure key management system
            // This is just a simplified example
            string key = Environment.GetEnvironmentVariable("SOSINTERNET_ENCRYPTION_KEY") ?? "SosInternetDefaultKey";

            // Simplified implementation for example
            // In production, use a more robust system with IV and salt
            using var aes = Aes.Create();
            var keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32)[..32]);
            aes.Key = keyBytes;
            aes.IV = new byte[16]; // Fixed IV for simplicity, in production use a random IV and save it

            var encryptedBytes = Convert.FromBase64String(encryptedPassword);
            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(encryptedBytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs);

            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password decryption");
            throw new InvalidOperationException("Unable to decrypt the password", ex);
        }
    }
}