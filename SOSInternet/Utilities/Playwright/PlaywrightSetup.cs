using Microsoft.Playwright;
using System.Runtime.InteropServices;

namespace SOSInternet.Utilities.Playwright;

public static class PlaywrightSetup
{
    /// <summary>  
    /// Gets the default path where Playwright installs browsers based on the operating system.  
    /// </summary>  
    public static string GetDefaultBrowsersPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ms-playwright");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library", "Caches", "ms-playwright");
        }
        else 
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".cache", "ms-playwright");
        }
    }

    /// <summary>  
    /// Checks if Chromium is installed by attempting to launch it.  
    /// </summary>  
    public static async Task<bool> IsChromiumInstalled()
    {
        try
        {
            using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync();
            await browser.CloseAsync();

            Console.WriteLine("Chromium is already installed and functional.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chromium verification failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>  
    /// Ensures that the required browsers are installed, installing them if necessary.  
    /// </summary>  
    /// <param name="forceInstall">If true, forces the installation of browsers even if they are already installed.</param>  
    /// <param name="onlyChromium">If true, installs only Chromium; otherwise, installs all browsers.</param>  
    public static async Task<bool> EnsureBrowsersInstalledAsync(bool forceInstall = false, bool onlyChromium = true)
    {
        try
        {
            if (!forceInstall)
            {
                Console.WriteLine("Checking browser installation for Playwright...");
                if (await IsChromiumInstalled())
                {
                    return true; 
                }
            }

            Console.WriteLine("Browsers not installed or forced installation. Proceeding with installation...");

            string[] args;
            if (onlyChromium)
            {
                Console.WriteLine("Installing only Chromium...");
                args = new[] { "install", "chromium" };
            }
            else
            {
                Console.WriteLine("Installing all browsers...");
                args = new[] { "install" };
            }

            const int maxRetries = 3;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                Console.WriteLine($"Installation attempt {attempt}/{maxRetries}...");

                var installExitCode = Microsoft.Playwright.Program.Main(args);

                if (installExitCode == 0)
                {
                    Console.WriteLine("Browsers installed successfully.");
                    return true;
                }

                Console.WriteLine($"Attempt {attempt} failed with exit code {installExitCode}");

                if (attempt < maxRetries)
                {
                    await Task.Delay(2000);
                }
            }

            Console.WriteLine("All installation attempts failed.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during browser installation: {ex.Message}");
            return false;
        }
    }

    /// <summary>  
    /// Removes the browsers installed by Playwright.  
    /// </summary>  
    public static bool UninstallBrowsers()
    {
        try
        {
            var browsersPath = GetDefaultBrowsersPath();
            Console.WriteLine($"Removing browsers installed at: {browsersPath}");

            if (Directory.Exists(browsersPath))
            {
                Directory.Delete(browsersPath, true);
                Console.WriteLine("Browsers removed successfully.");
                return true;
            }

            Console.WriteLine("No browsers found to remove.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during browser removal: {ex.Message}");
            return false;
        }
    }

    /// <summary>  
    /// Configures the page timeouts.  
    /// </summary>  
    /// <param name="page">The page instance to configure.</param>  
    public static void ConfigurePageTimeouts(IPage page)
    {
        page.SetDefaultTimeout(60000); // 1 minute  

        page.SetDefaultNavigationTimeout(60000); // 1 minute  
    }
}