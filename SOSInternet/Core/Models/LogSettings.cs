namespace SOSInternet.Core.Models;
 
/// <summary>  
/// Represents the settings for logging, including the log file path and retention period.  
/// </summary>  
public class LogSettings
{
    /// <summary>  
    /// Gets or sets the file path where logs will be stored.  
    /// </summary>  
    public string? Path { get; set; }

    /// <summary>  
    /// Gets or sets the number of days to retain log files.  
    /// </summary>  
    public int RetentionDays { get; set; }
}