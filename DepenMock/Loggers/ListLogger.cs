using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DepenMock.Loggers;

/// <summary>
/// Provides a logger implementation that stores log messages categorized by <see cref="LogLevel"/>.
/// </summary>
/// <remarks>This logger stores log messages in memory, organized by <see cref="LogLevel"/>. Each log level is
/// associated with a list of messages. It is primarily intended for scenarios where logs need to be captured and
/// analyzed during runtime, such as testing or debugging.</remarks>
/// <typeparam name="TLoggerType">The type associated with the logger, typically representing the category or context of the logs.</typeparam>
public class ListLogger<TLoggerType> : ILogger<TLoggerType>, ILogger
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListLogger"/> class.
    /// </summary>
    /// <remarks>The logger stores log messages categorized by <see cref="LogLevel"/> in a dictionary. Each log
    /// level is associated with a list of messages.</remarks>
    public ListLogger()
    {
        Logs = new Dictionary<LogLevel, List<string>>
        {
            { LogLevel.Trace, new List<string>() },
            { LogLevel.Debug, new List<string>() },
            { LogLevel.Information, new List<string>() },
            { LogLevel.Warning, new List<string>() },
            { LogLevel.Error, new List<string>() },
            { LogLevel.Critical, new List<string>() },
            { LogLevel.None, new List<string>() }
        };
    }

    /// <summary>
    /// Logs a message at the specified log level.
    /// </summary>
    /// <remarks>The <paramref name="formatter"/> function is used to generate the log message from the provided
    /// state and exception. Ensure that the <paramref name="formatter"/> is not <see langword="null"/> and produces a
    /// meaningful string representation.</remarks>
    /// <typeparam name="TState">The type of the state object to be logged.</typeparam>
    /// <param name="logLevel">The severity level of the log message.</param>
    /// <param name="eventId">The identifier for the event being logged.</param>
    /// <param name="state">The state object containing information to be logged.</param>
    /// <param name="exception">The exception associated with the log entry, if any. Can be <see langword="null"/>.</param>
    /// <param name="formatter">A function that formats the <paramref name="state"/> and <paramref name="exception"/> into a log message.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);

        LogMessage(logLevel, message);
    }

    private void LogMessage(LogLevel logLevel, string message)
    {
        if (!Logs.ContainsKey(logLevel))
        {
            Logs.Add(logLevel, new List<string>());
        }

        Logs[logLevel].Add(message);
    }
        
    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <remarks>This method is typically used to group log messages or operations within a specific context. The
    /// returned scope is a no-op implementation, meaning it does not perform any actual scoping behavior.</remarks>
    /// <typeparam name="TState">The type of the state to associate with the scope.</typeparam>
    /// <param name="state">The state object to associate with the scope. This can be used to provide contextual information.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the scope when disposed.</returns>
    public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

    /// <summary>
    /// Determines whether logging is enabled for the specified <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logLevel">The log level to check for enabled logging.</param>
    /// <returns><see langword="true"/> if logging is enabled for the specified <paramref name="logLevel"/>;  otherwise, <see
    /// langword="false"/>.</returns>
    public bool IsEnabled(LogLevel logLevel) => false;
        
    /// <summary>
    /// Returns the <see cref="IDictionary{TKey,TValue}"/> of messages that have been logged
    /// </summary>
    public IDictionary<LogLevel, List<string>> Logs { get; }
}