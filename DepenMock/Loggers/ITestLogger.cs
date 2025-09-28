using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace DepenMock.Loggers;

public interface ITestLogger : ILogger
{
    IDictionary<LogLevel, List<string>> Logs { get; }
    
    /// <summary>
    /// Clears all logged messages from all log levels.
    /// </summary>
    void Clear();
}