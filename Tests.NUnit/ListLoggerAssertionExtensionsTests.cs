using System;
using System.Collections.Generic;
using System.Linq;
using DepenMock;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests.NUnit;

[TestFixture]
public class ListLoggerAssertionExtensionsTests
{
    private ListLogger<TestClass> _logger;

    [SetUp]
    public void Setup()
    {
        _logger = new ListLogger<TestClass>();
    }

    [Test]
    public void CriticalLogs_ShouldReturnCriticalLevelLogs()
    {
        // Arrange
        var criticalMessage = "Critical error occurred";
        _logger.Log(LogLevel.Critical, new EventId(1), criticalMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Information, new EventId(2), "Info message", null, (state, ex) => state.ToString());

        // Act
        var criticalLogs = _logger.CriticalLogs();

        // Assert
        Assert.That(criticalLogs.Count, Is.EqualTo(1));
        Assert.That(criticalLogs.First(), Is.EqualTo(criticalMessage));
    }

    [Test]
    public void DebugLogs_ShouldReturnDebugLevelLogs()
    {
        // Arrange
        var debugMessage = "Debug information";
        _logger.Log(LogLevel.Debug, new EventId(1), debugMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Error, new EventId(2), "Error message", null, (state, ex) => state.ToString());

        // Act
        var debugLogs = _logger.DebugLogs();

        // Assert
        Assert.That(debugLogs.Count, Is.EqualTo(1));
        Assert.That(debugLogs.First(), Is.EqualTo(debugMessage));
    }

    [Test]
    public void ErrorLogs_ShouldReturnErrorLevelLogs()
    {
        // Arrange
        var errorMessage = "An error occurred";
        _logger.Log(LogLevel.Error, new EventId(1), errorMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Warning, new EventId(2), "Warning message", null, (state, ex) => state.ToString());

        // Act
        var errorLogs = _logger.ErrorLogs();

        // Assert
        Assert.That(errorLogs.Count, Is.EqualTo(1));
        Assert.That(errorLogs.First(), Is.EqualTo(errorMessage));
    }

    [Test]
    public void InformationLogs_ShouldReturnInformationLevelLogs()
    {
        // Arrange
        var infoMessage = "Information message";
        _logger.Log(LogLevel.Information, new EventId(1), infoMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Debug, new EventId(2), "Debug message", null, (state, ex) => state.ToString());

        // Act
        var infoLogs = _logger.InformationLogs();

        // Assert
        Assert.That(infoLogs.Count, Is.EqualTo(1));
        Assert.That(infoLogs.First(), Is.EqualTo(infoMessage));
    }

    [Test]
    public void TraceLogs_ShouldReturnTraceLevelLogs()
    {
        // Arrange
        var traceMessage = "Trace message";
        _logger.Log(LogLevel.Trace, new EventId(1), traceMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Critical, new EventId(2), "Critical message", null, (state, ex) => state.ToString());

        // Act
        var traceLogs = _logger.TraceLogs();

        // Assert
        Assert.That(traceLogs.Count, Is.EqualTo(1));
        Assert.That(traceLogs.First(), Is.EqualTo(traceMessage));
    }

    [Test]
    public void WarningLogs_ShouldReturnWarningLevelLogs()
    {
        // Arrange
        var warningMessage = "Warning message";
        _logger.Log(LogLevel.Warning, new EventId(1), warningMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Information, new EventId(2), "Info message", null, (state, ex) => state.ToString());

        // Act
        var warningLogs = _logger.WarningLogs();

        // Assert
        Assert.That(warningLogs.Count, Is.EqualTo(1));
        Assert.That(warningLogs.First(), Is.EqualTo(warningMessage));
    }

    [Test]
    public void ContainsMessage_WithMatchingMessage_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "First message", "Second message", "Third message" };
        var searchFragment = "Second";

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => messages.ContainsMessage(searchFragment));
    }

    [Test]
    public void ContainsMessage_WithCaseInsensitiveMatch_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "First Message", "SECOND MESSAGE", "third message" };
        var searchFragment = "second";

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => messages.ContainsMessage(searchFragment));
    }

    [Test]
    public void ContainsMessage_WithNonMatchingMessage_ShouldThrowException()
    {
        // Arrange
        var messages = new List<string> { "First message", "Second message", "Third message" };
        var searchFragment = "Fourth";

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => messages.ContainsMessage(searchFragment));
        Assert.That(exception.Message, Does.Contain(searchFragment));
    }

    [Test]
    public void ContainsMessage_WithEmptyList_ShouldThrowException()
    {
        // Arrange
        var messages = new List<string>();
        var searchFragment = "Any message";

        // Act & Assert
        Assert.Throws<Exception>(() => messages.ContainsMessage(searchFragment));
    }

    [Test]
    public void ContainsMessage_WithPartialMatch_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "This is a complete message" };
        var searchFragment = "complete";

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => messages.ContainsMessage(searchFragment));
    }

    [Test]
    public void AssertContains_WithMatchingMessage_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "First message", "Second message", "Third message" };
        var searchFragment = "Second";

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => messages.AssertContains(searchFragment));
    }

    [Test]
    public void AssertContains_WithCaseInsensitiveMatch_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "First Message", "SECOND MESSAGE", "third message" };
        var searchFragment = "second";

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => messages.AssertContains(searchFragment));
    }

    [Test]
    public void AssertContains_WithNonMatchingMessage_ShouldThrowException()
    {
        // Arrange
        var messages = new List<string> { "First message", "Second message", "Third message" };
        var searchFragment = "Fourth";

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => messages.AssertContains(searchFragment));
        Assert.That(exception.Message, Does.Contain(searchFragment));
    }

    [Test]
    public void AssertContains_WithEmptyList_ShouldThrowException()
    {
        // Arrange
        var messages = new List<string>();
        var searchFragment = "Any message";

        // Act & Assert
        Assert.Throws<Exception>(() => messages.AssertContains(searchFragment));
    }

    [Test]
    public void AssertContains_WithPartialMatch_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "This is a complete message" };
        var searchFragment = "complete";

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => messages.AssertContains(searchFragment));
    }

    private class TestClass { }
}