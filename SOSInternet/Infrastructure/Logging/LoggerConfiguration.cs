using NLog.Config;
using NLog.Targets;
using NLog;
using System.IO;
using NLog.Conditions;

namespace SOSInternet.Infrastructure.Logging
{
    /// <summary>
    /// Classe per la configurazione del logger
    /// </summary>
    public static class LoggerConfiguration
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Configura il logger dell'applicazione
        /// </summary>
        public static void ConfigureLogger()
        {
            var config = new LoggingConfiguration();

            // Crea la directory dei log se non esiste
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Target per file giornaliero
            var fileTarget = new FileTarget("file")
            {
                // ${basedir} è la directory dell'applicazione
                // ${shortdate} è la data in formato yyyy-MM-dd
                FileName = "${basedir}/logs/${shortdate}.log",
                
                // Layout del log
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}",
                
                // Configurazione encoding
                Encoding = System.Text.Encoding.UTF8,
                
                // Configurazione archivio
                ArchiveFileName = "${basedir}/logs/archive/{#}.log",
                ArchiveNumbering = ArchiveNumberingMode.Date,
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveDateFormat = "yyyy-MM-dd",
                MaxArchiveFiles = 30, // Mantiene i log degli ultimi 30 giorni
                
                // Buffer writing per migliorare le performance
                BufferSize = 16384, // 16KB
                AutoFlush = true,
                
                // Crea la directory se non esiste
                CreateDirs = true,
                
                // Gestione concorrenza
                ConcurrentWrites = true,
                KeepFileOpen = false
            };

            // Target per console
            var consoleTarget = new ColoredConsoleTarget("console")
            {
                Layout = "${time}|${level:uppercase=true}|${message}"
            };

            // Configurazione colori per la console
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

            // Aggiungi i target alla configurazione
            config.AddTarget(fileTarget);
            config.AddTarget(consoleTarget);

            // Regole di logging
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, "console"); // Log tutto sulla console
            config.AddRule(LogLevel.Info, LogLevel.Fatal, "file");     // Log su file solo da Info in su

            // Applica la configurazione
            LogManager.Configuration = config;
            
            Logger.Info("Logger configurato con successo");
        }

        /// <summary>
        /// Ottiene il logger per la classe corrente
        /// </summary>
        /// <returns>Logger configurato</returns>
        public static ILogger GetLogger()
        {
            return Logger;
        }
    }
}
