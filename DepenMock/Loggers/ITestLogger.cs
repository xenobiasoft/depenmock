using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace DepenMock.Loggers;

public interface ITestLogger : ILogger
{
    IDictionary<LogLevel, List<string>> Logs { get; }
}