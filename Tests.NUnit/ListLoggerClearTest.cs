using System;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests.NUnit;

[TestFixture]
public class ListLoggerClearTest
{
    private ListLogger<TestClass> _logger;

    [SetUp]
    public void Setup()
    {
        _logger = new ListLogger<TestClass>();
    }

    [Test]
    public void Clear_ShouldRemoveAllLogMessages()
    {
        // Arrange
        _logger.Log(LogLevel.Information, new EventId(1), "Info message", null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Error, new EventId(2), "Error message", null, (state, ex) => state.ToString());
        
        Assert.That(_logger.Logs[LogLevel.Information].Count, Is.EqualTo(1));
        Assert.That(_logger.Logs[LogLevel.Error].Count, Is.EqualTo(1));

        // Act
        _logger.Clear();

        // Assert
        Assert.That(_logger.Logs[LogLevel.Information].Count, Is.EqualTo(0));
        Assert.That(_logger.Logs[LogLevel.Error].Count, Is.EqualTo(0));
        Assert.That(_logger.Logs[LogLevel.Warning].Count, Is.EqualTo(0));
    }

    [Test]
    public void Clear_WithEmptyLogs_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => _logger.Clear());
    }

    private class TestClass { }
}