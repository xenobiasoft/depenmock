using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Tests.XUnit;

public class ListLoggerTests
{
    private readonly ListLogger<TestClass> _logger;

    public ListLoggerTests()
    {
        _logger = new ListLogger<TestClass>();
    }

    [Fact]
    public void Constructor_ShouldInitializeLogsWithAllLogLevels()
    {
        // Assert
        Assert.NotNull(_logger.Logs);
        Assert.True(_logger.Logs.ContainsKey(LogLevel.Trace));
        Assert.True(_logger.Logs.ContainsKey(LogLevel.Debug));
        Assert.True(_logger.Logs.ContainsKey(LogLevel.Information));
        Assert.True(_logger.Logs.ContainsKey(LogLevel.Warning));
        Assert.True(_logger.Logs.ContainsKey(LogLevel.Error));
        Assert.True(_logger.Logs.ContainsKey(LogLevel.Critical));
        Assert.True(_logger.Logs.ContainsKey(LogLevel.None));
    }

    [Fact]
    public void Log_ShouldAddMessageToCorrectLogLevel()
    {
        // Arrange
        var testMessage = "Test message";
        var logLevel = LogLevel.Information;

        // Act
        _logger.Log(logLevel, new EventId(1), testMessage, null, (state, ex) => state.ToString());

        // Assert
        Assert.Equal(1, _logger.Logs[logLevel].Count);
        Assert.Equal(testMessage, _logger.Logs[logLevel].First());
    }

    [Fact]
    public void Log_WithException_ShouldFormatMessageWithException()
    {
        // Arrange
        var testMessage = "Test message";
        var exception = new InvalidOperationException("Test exception");
        var logLevel = LogLevel.Error;

        // Act
        _logger.Log(logLevel, new EventId(1), testMessage, exception, 
            (state, ex) => $"{state} - Exception: {ex?.Message}");

        // Assert
        Assert.Equal(1, _logger.Logs[logLevel].Count);
        Assert.Contains(testMessage, _logger.Logs[logLevel].First());
        Assert.Contains(exception.Message, _logger.Logs[logLevel].First());
    }

    [Fact]
    public void Log_MultipleMessages_ShouldAddAllMessages()
    {
        // Arrange
        var messages = new[] { "Message 1", "Message 2", "Message 3" };
        var logLevel = LogLevel.Debug;

        // Act
        foreach (var message in messages)
        {
            _logger.Log(logLevel, new EventId(1), message, null, (state, ex) => state.ToString());
        }

        // Assert
        Assert.Equal(3, _logger.Logs[logLevel].Count);
        for (int i = 0; i < messages.Length; i++)
        {
            Assert.Equal(messages[i], _logger.Logs[logLevel][i]);
        }
    }

    [Fact]
    public void Log_NewLogLevel_ShouldCreateNewListForLevel()
    {
        // Arrange
        var customLogLevel = (LogLevel)99;
        var testMessage = "Custom level message";

        // Act
        _logger.Log(customLogLevel, new EventId(1), testMessage, null, (state, ex) => state.ToString());

        // Assert
        Assert.True(_logger.Logs.ContainsKey(customLogLevel));
        Assert.Equal(1, _logger.Logs[customLogLevel].Count);
        Assert.Equal(testMessage, _logger.Logs[customLogLevel].First());
    }

    [Fact]
    public void BeginScope_ShouldReturnNullScope()
    {
        // Act
        var scope = _logger.BeginScope("test state");

        // Assert
        Assert.NotNull(scope);
    }

    [Fact]
    public void IsEnabled_ShouldAlwaysReturnFalse()
    {
        // Act & Assert
        Assert.False(_logger.IsEnabled(LogLevel.Trace));
        Assert.False(_logger.IsEnabled(LogLevel.Debug));
        Assert.False(_logger.IsEnabled(LogLevel.Information));
        Assert.False(_logger.IsEnabled(LogLevel.Warning));
        Assert.False(_logger.IsEnabled(LogLevel.Error));
        Assert.False(_logger.IsEnabled(LogLevel.Critical));
        Assert.False(_logger.IsEnabled(LogLevel.None));
    }

    private class TestClass { }
}