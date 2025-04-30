#  SOSInternet

## Overview

SOSInternet is a .NET console application designed to monitor the internet connection and automatically reboot a TP-Link router when the connection drops. The application is particularly useful in situations involving large downloads over extended periods, which can cause the router to freeze.

## Architecture

The application was designed following SOLID principles and utilizes the Dependency Injection pattern to ensure a modular, testable, and easily extensible structure.

### Project Structure

```
SOSInternet/
├── src/
│   ├── SOSInternet/
│   │   ├── Program.cs                     # Application entry point
│   │   ├── appsettings.json               # Configuration file
│   │   ├── Core/
│   │   │   ├── Interfaces/                # Interfaces for services
│   │   │   ├── Models/                    # Data models
│   │   │   └── Services/                  # Service implementations
│   │   ├── Infrastructure/
│   │   │   ├── Configuration/             # Configuration management
│   │   │   └── Logging/                   # Logger configuration
│   │   └── Utilities/
│   │       ├── Extensions/                # Service extensions
│   │       └── Playwright/                # Playwright Setup
├── docs/
│   ├── README.md                          # Main documentation
└── .gitignore                             # .gitignore file for GitHub
```

### Main Components

#### Core

The `Core` namespace contains the application's business logic:

1.  **Interfaces**: Defines the contracts for the services
    *   `IInternetChecker`: Interface for checking the internet connection
    *   `IRouterRebooter`: Interface for rebooting the router
    *   `IConnectionMonitor`: Interface for monitoring the connection

2.  **Models**: Defines the data models
    *   `ConnectionStatus`: Model for the connection status
    *   `RouterSettings`: Model for the router settings
    *   `ConnectionSettings`: Model for the connection settings

3.  **Services**: Implements the business logic
    *   `InternetChecker`: Implementation for checking the connection
    *   `TpLinkRouterRebooter`: Implementation for rebooting the TP-Link router
    *   `ConnectionMonitor`: Implementation for monitoring the connection

#### Infrastructure

The `Infrastructure` namespace manages the application's infrastructure:

1.  **Configuration**: Manages the application's configurations
    *   `AppSettings`: Class for configuration management

2.  **Logging**: Configures and manages logging
    *   `LoggerConfiguration`: Class for logger configuration

#### Utilities

The `Utilities` namespace contains utilities and extensions:

1.  **Extensions**: Extensions to simplify service configuration
    *   `ServiceCollectionExtensions`: Extensions for service registration

## Application Flow

1.  The application is started by `Program.cs`
2.  Configurations are loaded from `appsettings.json`
3.  The logger is configured
4.  Services are registered via Dependency Injection
5.  The `ConnectionMonitor` is started, which:
    *   Periodically checks the internet connection via `InternetChecker`
    *   If the connection is lost, it attempts to restore it
    *   After a configurable number of failed attempts, it reboots the router via `TpLinkRouterRebooter`
    *   Continues monitoring after the reboot

## Configuration

The application's configurations are managed through the `appsettings.json` file:

```json
{
  "RouterSettings": {
    "Url": "http://192.168.1.1",
    "Username": "admin",
    "Password": "password",
    "UseEncryption": false
  },
  "ConnectionSettings": {
    "CheckIntervalSeconds": 15,
    "RetryAttemptsBeforeReboot": 4,
    "WaitAfterRebootSeconds": 120,
    "PingTargets": [
      "8.8.8.8",
      "1.1.1.1",
      "google.com"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    },
    "FileLogging": {
      "Enabled": true,
      "Path": "logs",
      "RetentionDays": 30
    }
  }
}
```

### RouterSettings

*   `Url`: Router administration URL
*   `Username`: Username for router access (if necessary)
*   `Password`: Password for router access
*   `UseEncryption`: Indicates if the password is encrypted

### ConnectionSettings

*   `CheckIntervalSeconds`: Interval in seconds between connection checks
*   `RetryAttemptsBeforeReboot`: Number of attempts before rebooting the router
*   `WaitAfterRebootSeconds`: Waiting time in seconds after router reboot
*   `PingTargets`: List of addresses to ping to verify the connection

### Logging

*   `LogLevel`: Logging levels for different categories
*   `FileLogging`: Configuration for file logging

## Security

The application implements the following security measures:

*   Sensitive credentials can be encrypted in the configuration file
*   A secure decryption mechanism is implemented
*   No credentials are hardcoded in the source code

## Logging

The application uses NLog for structured logging:

*   Logs are written to both console and file
*   Log files are archived daily
*   Logs are retained for a configurable period (default: 30 days)
*   Console logs are colored based on the logging level

## Extensibility

The architecture is designed to be easily extensible:

*   Support for different router types via the `IRouterRebooter` interface
*   Support for different connection checking methods via the `IInternetChecker` interface
*   Ability to add new features without modifying existing code

## System Requirements

*   .NET 8.0 or higher
*   Administrative access to the TP-Link router
*   Working network connection

## Dependencies

*   Microsoft.Extensions.Hosting
*   Microsoft.Extensions.DependencyInjection
*   Microsoft.Extensions.Configuration
*   Microsoft.Extensions.Logging
*   Microsoft.Playwright
*   NLog.Extensions.Logging

## Installation

1.  Clone the repository
2.  Configure the `appsettings.json` file with your router settings
3.  Build the application with `dotnet build`
4.  Run the application with `dotnet run`

## Usage

The application is designed to run in the background and does not require user interaction. It can be configured for automatic startup on system boot using Windows Task Scheduler or a system service.

## Troubleshooting

### Logs

Application logs are available in the `logs` directory and can be used to diagnose any issues.

### Common Issues

1.  **Application cannot access the router**
    *   Verify that the router URL is correct
    *   Verify that the login credentials are correct
    *   Verify that the router is reachable from the network

2.  **Application does not detect connection loss**
    *   Verify that the ping targets are reachable
    *   Increase the number of attempts before reboot

3.  **Application fails to reboot the router**
    *   Verify that the login credentials are correct
    *   Verify that the router supports rebooting via the web interface
    *   Verify that the router's web interface has not changed

## Contributing

Contributions are welcome! To contribute to the project:

1.  Fork the repository
2.  Create a branch for your feature (`git checkout -b feature/AmazingFeature`)
3.  Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push the branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

## License

This project is distributed under the MIT License. 
