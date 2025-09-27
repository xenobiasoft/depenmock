using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MSTest;

[TestClass]
public class ListLoggerTests
{
    private ListLogger<TestClass> _logger;

    [TestInitialize]
    public void Setup()
    {
        _logger = new ListLogger<TestClass>();
    }

    [TestMethod]
    public void Constructor_ShouldInitializeLogsWithAllLogLevels()
    {
        // Assert
        Assert.IsNotNull(_logger.Logs);
        Assert.IsTrue(_logger.Logs.ContainsKey(LogLevel.Trace));
        Assert.IsTrue(_logger.Logs.ContainsKey(LogLevel.Debug));
        Assert.IsTrue(_logger.Logs.ContainsKey(LogLevel.Information));
        Assert.IsTrue(_logger.Logs.ContainsKey(LogLevel.Warning));
        Assert.IsTrue(_logger.Logs.ContainsKey(LogLevel.Error));
        Assert.IsTrue(_logger.Logs.ContainsKey(LogLevel.Critical));
        Assert.IsTrue(_logger.Logs.ContainsKey(LogLevel.None));
    }

    [TestMethod]
    public void Log_ShouldAddMessageToCorrectLogLevel()
    {
        // Arrange
        var testMessage = "Test message";
        var logLevel = LogLevel.Information;

        // Act
        _logger.Log(logLevel, new EventId(1), testMessage, null, (state, ex) => state.ToString());

        // Assert
        Assert.AreEqual(1, _logger.Logs[logLevel].Count);
        Assert.AreEqual(testMessage, _logger.Logs[logLevel].First());
    }

    [TestMethod]
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
        Assert.AreEqual(1, _logger.Logs[logLevel].Count);
        Assert.IsTrue(_logger.Logs[logLevel].First().Contains(testMessage));
        Assert.IsTrue(_logger.Logs[logLevel].First().Contains(exception.Message));
    }

    [TestMethod]
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
        Assert.AreEqual(3, _logger.Logs[logLevel].Count);
        for (int i = 0; i < messages.Length; i++)
        {
            Assert.AreEqual(messages[i], _logger.Logs[logLevel][i]);
        }
    }

    [TestMethod]
    public void Log_NewLogLevel_ShouldCreateNewListForLevel()
    {
        // Arrange
        var customLogLevel = (LogLevel)99;
        var testMessage = "Custom level message";

        // Act
        _logger.Log(customLogLevel, new EventId(1), testMessage, null, (state, ex) => state.ToString());

        // Assert
        Assert.IsTrue(_logger.Logs.ContainsKey(customLogLevel));
        Assert.AreEqual(1, _logger.Logs[customLogLevel].Count);
        Assert.AreEqual(testMessage, _logger.Logs[customLogLevel].First());
    }

    [TestMethod]
    public void BeginScope_ShouldReturnNullScope()
    {
        // Act
        var scope = _logger.BeginScope("test state");

        // Assert
        Assert.IsNotNull(scope);
    }

    [TestMethod]
    public void IsEnabled_ShouldAlwaysReturnFalse()
    {
        // Act & Assert
        Assert.IsFalse(_logger.IsEnabled(LogLevel.Trace));
        Assert.IsFalse(_logger.IsEnabled(LogLevel.Debug));
        Assert.IsFalse(_logger.IsEnabled(LogLevel.Information));
        Assert.IsFalse(_logger.IsEnabled(LogLevel.Warning));
        Assert.IsFalse(_logger.IsEnabled(LogLevel.Error));
        Assert.IsFalse(_logger.IsEnabled(LogLevel.Critical));
        Assert.IsFalse(_logger.IsEnabled(LogLevel.None));
    }

    private class TestClass { }
}