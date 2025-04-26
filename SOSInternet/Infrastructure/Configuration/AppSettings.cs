using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SOSInternet.Core.Models;
using SOSInternet.Infrastructure.Logging;
using SOSInternet.Utilities.Extensions;

namespace SOSInternet.Infrastructure.Configuration;

/// <summary>  
/// Class for managing application configurations.  
/// </summary>  
public static class AppSettings
{
    /// <summary>  
    /// Configures the application services.  
    /// </summary>  
    /// <param name="services">Service collection.</param>  
    /// <param name="configuration">Application configuration.</param>  
    /// <returns>Configured service collection.</returns>  
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
            loggingBuilder.AddNLog();
        });
 
        LoggerConfiguration.ConfigureLogger(configuration.GetSection("FileLogging"));
 
        services.Configure<RouterSettings>(configuration.GetSection("RouterSettings"));
        services.Configure<ConnectionSettings>(configuration.GetSection("ConnectionSettings"));
  
        services.AddApplicationServices();
         

        return services;
    }

    /// <summary>  
    /// Creates the application configuration.  
    /// </summary>  
    /// <param name="args">Command-line arguments.</param>  
    /// <returns>Application configuration.</returns>  
    public static IConfiguration CreateConfiguration(string[] args)
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
    }
}