using NLog.Config;
using NLog.Targets;
using NLog;
using System.IO;
using Microsoft.Extensions.Configuration;
using NLog.Conditions;
using SOSInternet.Core.Models;

namespace SOSInternet.Infrastructure.Logging;

/// <summary>  
/// Class for configuring the logger  
/// </summary>  
public static class LoggerConfiguration
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>  
    /// Configures the application logger  
    /// </summary>  
    public static void ConfigureLogger(IConfiguration configuration)
    {
        var logSettings=configuration.Get<LogSettings>();

        var config = new LoggingConfiguration();
        
        var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), logSettings?.Path ?? "logs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        } 
 
        var fileTarget = new FileTarget("file")
        {
            FileName = "${basedir}/logs/${shortdate}.log",

            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}",

            Encoding = System.Text.Encoding.UTF8,

            ArchiveFileName = "${basedir}/logs/archive/{#}.log",
            ArchiveNumbering = ArchiveNumberingMode.Date,
            ArchiveEvery = FileArchivePeriod.Day,
            ArchiveDateFormat = "yyyy-MM-dd",
            MaxArchiveFiles = logSettings?.RetentionDays ?? 30,  

            BufferSize = 16384, // 16KB  
            AutoFlush = true,

            CreateDirs = true,

            ConcurrentWrites = true,
            KeepFileOpen = false
        };

        var consoleTarget = new ColoredConsoleTarget("console")
        {
            Layout = "${time}|${level:uppercase=true}|${message}"
        };

        consoleTarget.RowHighlightingRules.Add(
            new ConsoleRowHighlightingRule(
                condition: ConditionParser.ParseExpression("level == LogLevel.Debug"),
                foregroundColor: ConsoleOutputColor.Gray,
                backgroundColor: ConsoleOutputColor.Black));

        consoleTarget.RowHighlightingRules.Add(
            new ConsoleRowHighlightingRule(
                condition: ConditionParser.ParseExpression("level == LogLevel.Info"),
                foregroundColor: ConsoleOutputColor.White,
                backgroundColor: ConsoleOutputColor.Black));

        consoleTarget.RowHighlightingRules.Add(
            new ConsoleRowHighlightingRule(
                condition: ConditionParser.ParseExpression("level == LogLevel.Warn"),
                foregroundColor: ConsoleOutputColor.Yellow,
                backgroundColor: ConsoleOutputColor.Black));

        consoleTarget.RowHighlightingRules.Add(
            new ConsoleRowHighlightingRule(
                condition: ConditionParser.ParseExpression("level == LogLevel.Error"),
                foregroundColor: ConsoleOutputColor.Red,
                backgroundColor: ConsoleOutputColor.Black));

        config.AddTarget(fileTarget);
        config.AddTarget(consoleTarget);

        config.AddRule(LogLevel.Debug, LogLevel.Fatal, "console");
        config.AddRule(LogLevel.Info, LogLevel.Fatal, "file");

        LogManager.Configuration = config;

        Logger.Info("Logger successfully configured");
    }

    /// <summary>  
    /// Gets the logger for the current class  
    /// </summary>  
    /// <returns>Configured logger</returns>  
    public static ILogger GetLogger()
    {
        return Logger;
    }
}
