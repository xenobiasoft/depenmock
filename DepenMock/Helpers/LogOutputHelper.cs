using System;
using System.Linq;
using System.Reflection;
using System.Text;
using DepenMock.Attributes;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;

namespace DepenMock.Helpers;

/// <summary>
/// Provides helper methods for formatting and outputting log messages from test loggers.
/// </summary>
public static class LogOutputHelper
{
    /// <summary>
    /// Formats log messages from the test logger into a readable string format.
    /// </summary>
    /// <param name="logger">The test logger containing the log messages.</param>
    /// <returns>A formatted string containing all log messages, or null if no messages exist.</returns>
    public static string FormatLogMessages(ITestLogger logger)
    {
        if (logger?.Logs == null || !logger.Logs.Any(kvp => kvp.Value.Any()))
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.AppendLine("=== Test Log Messages ===");

        foreach (var logLevel in Enum.GetValues<LogLevel>().Where(l => l != LogLevel.None))
        {
            if (logger.Logs.TryGetValue(logLevel, out var messages) && messages.Any())
            {
                sb.AppendLine($"[{logLevel}]");
                foreach (var message in messages)
                {
                    sb.AppendLine($"  {message}");
                }
                sb.AppendLine();
            }
        }

        return sb.Length > "=== Test Log Messages ===\r\n".Length ? sb.ToString() : null;
    }

    /// <summary>
    /// Determines if log output should occur based on the attribute configuration and test result.
    /// </summary>
    /// <param name="method">The test method to check for log output attribute.</param>
    /// <param name="testClass">The test class to check for log output attribute.</param>
    /// <param name="testPassed">Whether the test passed or failed.</param>
    /// <returns>True if log output should occur, false otherwise.</returns>
    public static bool ShouldOutputLogs(MethodInfo method, Type testClass, bool testPassed)
    {
        // First check method level attribute
        var methodAttribute = method?.GetCustomAttribute<LogOutputAttribute>();
        if (methodAttribute != null)
        {
            return ShouldOutputForTiming(methodAttribute.Timing, testPassed);
        }

        // Then check class level attribute
        var classAttribute = testClass?.GetCustomAttribute<LogOutputAttribute>();
        if (classAttribute != null)
        {
            return ShouldOutputForTiming(classAttribute.Timing, testPassed);
        }

        // No attribute found, don't output logs
        return false;
    }

    /// <summary>
    /// Determines if logs should be output based on timing configuration and test result.
    /// </summary>
    /// <param name="timing">The timing configuration.</param>
    /// <param name="testPassed">Whether the test passed.</param>
    /// <returns>True if logs should be output, false otherwise.</returns>
    private static bool ShouldOutputForTiming(LogOutputTiming timing, bool testPassed)
    {
        return timing switch
        {
            LogOutputTiming.Always => true,
            LogOutputTiming.OnFailure => !testPassed,
            LogOutputTiming.OnSuccess => testPassed,
            _ => false
        };
    }
}