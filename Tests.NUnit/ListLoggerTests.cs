using System;
using System.Linq;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests.NUnit;

[TestFixture]
public class ListLoggerTests
{
    private ListLogger<TestClass> _logger;

    [SetUp]
    public void Setup()
    {
        _logger = new ListLogger<TestClass>();
    }

    [Test]
    public void Constructor_ShouldInitializeLogsWithAllLogLevels()
    {
        // Assert
        Assert.That(_logger.Logs, Is.Not.Null);
        Assert.That(_logger.Logs.ContainsKey(LogLevel.Trace), Is.True);
        Assert.That(_logger.Logs.ContainsKey(LogLevel.Debug), Is.True);
        Assert.That(_logger.Logs.ContainsKey(LogLevel.Information), Is.True);
        Assert.That(_logger.Logs.ContainsKey(LogLevel.Warning), Is.True);
        Assert.That(_logger.Logs.ContainsKey(LogLevel.Error), Is.True);
        Assert.That(_logger.Logs.ContainsKey(LogLevel.Critical), Is.True);
        Assert.That(_logger.Logs.ContainsKey(LogLevel.None), Is.True);
    }

    [Test]
    public void Log_ShouldAddMessageToCorrectLogLevel()
    {
        // Arrange
        var testMessage = "Test message";
        var logLevel = LogLevel.Information;

        // Act
        _logger.Log(logLevel, new EventId(1), testMessage, null, (state, ex) => state.ToString());

        // Assert
        Assert.That(_logger.Logs[logLevel].Count, Is.EqualTo(1));
        Assert.That(_logger.Logs[logLevel].First(), Is.EqualTo(testMessage));
    }

    [Test]
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
        Assert.That(_logger.Logs[logLevel].Count, Is.EqualTo(1));
        Assert.That(_logger.Logs[logLevel].First(), Does.Contain(testMessage));
        Assert.That(_logger.Logs[logLevel].First(), Does.Contain(exception.Message));
    }

    [Test]
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
        Assert.That(_logger.Logs[logLevel].Count, Is.EqualTo(3));
        for (int i = 0; i < messages.Length; i++)
        {
            Assert.That(_logger.Logs[logLevel][i], Is.EqualTo(messages[i]));
        }
    }

    [Test]
    public void Log_NewLogLevel_ShouldCreateNewListForLevel()
    {
        // Arrange
        var customLogLevel = (LogLevel)99;
        var testMessage = "Custom level message";

        // Act
        _logger.Log(customLogLevel, new EventId(1), testMessage, null, (state, ex) => state.ToString());

        // Assert
        Assert.That(_logger.Logs.ContainsKey(customLogLevel), Is.True);
        Assert.That(_logger.Logs[customLogLevel].Count, Is.EqualTo(1));
        Assert.That(_logger.Logs[customLogLevel].First(), Is.EqualTo(testMessage));
    }

    [Test]
    public void BeginScope_ShouldReturnNullScope()
    {
        // Act
        var scope = _logger.BeginScope("test state");

        // Assert
        Assert.That(scope, Is.Not.Null);
    }

    [Test]
    public void IsEnabled_ShouldAlwaysReturnFalse()
    {
        // Act & Assert
        Assert.That(_logger.IsEnabled(LogLevel.Trace), Is.False);
        Assert.That(_logger.IsEnabled(LogLevel.Debug), Is.False);
        Assert.That(_logger.IsEnabled(LogLevel.Information), Is.False);
        Assert.That(_logger.IsEnabled(LogLevel.Warning), Is.False);
        Assert.That(_logger.IsEnabled(LogLevel.Error), Is.False);
        Assert.That(_logger.IsEnabled(LogLevel.Critical), Is.False);
        Assert.That(_logger.IsEnabled(LogLevel.None), Is.False);
    }

    private class TestClass { }
}