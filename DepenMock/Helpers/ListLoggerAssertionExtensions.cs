using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DepenMock;

/// <summary>
/// Provides extension methods for <see cref="ListLogger{T}"/> to facilitate log assertions in testing scenarios.
/// </summary>
public static class ListLoggerAssertionExtensions
{
    /// <summary>
    /// Gets all critical level log messages from the logger.
    /// </summary>
    /// <typeparam name="T">The type associated with the logger.</typeparam>
    /// <param name="logger">The logger instance to extract messages from.</param>
    /// <returns>A list of critical log messages.</returns>
    public static List<string> CriticalLogs<T>(this ListLogger<T> logger)
    {
        return logger.ErrorLogs(LogLevel.Critical);
    }

    /// <summary>
    /// Gets all debug level log messages from the logger.
    /// </summary>
    /// <typeparam name="T">The type associated with the logger.</typeparam>
    /// <param name="logger">The logger instance to extract messages from.</param>
    /// <returns>A list of debug log messages.</returns>
    public static List<string> DebugLogs<T>(this ListLogger<T> logger)
    {
        return logger.ErrorLogs(LogLevel.Debug);
    }

    /// <summary>
    /// Gets all error level log messages from the logger.
    /// </summary>
    /// <typeparam name="T">The type associated with the logger.</typeparam>
    /// <param name="logger">The logger instance to extract messages from.</param>
    /// <returns>A list of error log messages.</returns>
    public static List<string> ErrorLogs<T>(this ListLogger<T> logger)
    {
        return logger.ErrorLogs(LogLevel.Error);
    }

    /// <summary>
    /// Gets all information level log messages from the logger.
    /// </summary>
    /// <typeparam name="T">The type associated with the logger.</typeparam>
    /// <param name="logger">The logger instance to extract messages from.</param>
    /// <returns>A list of information log messages.</returns>
    public static List<string> InformationLogs<T>(this ListLogger<T> logger)
    {
        return logger.ErrorLogs(LogLevel.Information);
    }

    /// <summary>
    /// Gets all trace level log messages from the logger.
    /// </summary>
    /// <typeparam name="T">The type associated with the logger.</typeparam>
    /// <param name="logger">The logger instance to extract messages from.</param>
    /// <returns>A list of trace log messages.</returns>
    public static List<string> TraceLogs<T>(this ListLogger<T> logger)
    {
        return logger.ErrorLogs(LogLevel.Trace);
    }

    /// <summary>
    /// Gets all warning level log messages from the logger.
    /// </summary>
    /// <typeparam name="T">The type associated with the logger.</typeparam>
    /// <param name="logger">The logger instance to extract messages from.</param>
    /// <returns>A list of warning log messages.</returns>
    public static List<string> WarningLogs<T>(this ListLogger<T> logger)
    {
        return logger.ErrorLogs(LogLevel.Warning);
    }

    /// <summary>
    /// Asserts that at least one log message contains the specified text fragment.
    /// </summary>
    /// <param name="logMessages">The list of log messages to check.</param>
    /// <param name="messageFragment">The text fragment to search for in the log messages.</param>
    /// <exception cref="Exception">Thrown when no log message contains the specified fragment.</exception>
    public static void ContainsMessage(this List<string> logMessages, string messageFragment)
    {
        if (!logMessages.Any(log => log.Contains(messageFragment, StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exception($"No log message contains the message: '{messageFragment}'");
        }
    }

    /// <summary>
    /// Helper method to extract log messages of a specific level from the logger.
    /// </summary>
    /// <typeparam name="T">The type associated with the logger.</typeparam>
    /// <param name="logger">The logger instance to extract messages from.</param>
    /// <param name="logLevel">The log level to filter by.</param>
    /// <returns>A list of log messages for the specified log level.</returns>
    private static List<string> ErrorLogs<T>(this ListLogger<T> logger, LogLevel logLevel)
    {
        return logger.Logs[logLevel].ToList();
    }
}