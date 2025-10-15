using DepenMock;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MSTest;

[TestClass]
public class ListLoggerAssertionExtensionsTests
{
    private ListLogger<TestClass> _logger;

    [TestInitialize]
    public void Setup()
    {
        _logger = new ListLogger<TestClass>();
    }

    [TestMethod]
    public void CriticalLogs_ShouldReturnCriticalLevelLogs()
    {
        // Arrange
        var criticalMessage = "Critical error occurred";
        _logger.Log(LogLevel.Critical, new EventId(1), criticalMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Information, new EventId(2), "Info message", null, (state, ex) => state.ToString());

        // Act
        var criticalLogs = _logger.CriticalLogs();

        // Assert
        Assert.AreEqual(1, criticalLogs.Count);
        Assert.AreEqual(criticalMessage, criticalLogs.First());
    }

    [TestMethod]
    public void DebugLogs_ShouldReturnDebugLevelLogs()
    {
        // Arrange
        var debugMessage = "Debug information";
        _logger.Log(LogLevel.Debug, new EventId(1), debugMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Error, new EventId(2), "Error message", null, (state, ex) => state.ToString());

        // Act
        var debugLogs = _logger.DebugLogs();

        // Assert
        Assert.AreEqual(1, debugLogs.Count);
        Assert.AreEqual(debugMessage, debugLogs.First());
    }

    [TestMethod]
    public void ErrorLogs_ShouldReturnErrorLevelLogs()
    {
        // Arrange
        var errorMessage = "An error occurred";
        _logger.Log(LogLevel.Error, new EventId(1), errorMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Warning, new EventId(2), "Warning message", null, (state, ex) => state.ToString());

        // Act
        var errorLogs = _logger.ErrorLogs();

        // Assert
        Assert.AreEqual(1, errorLogs.Count);
        Assert.AreEqual(errorMessage, errorLogs.First());
    }

    [TestMethod]
    public void InformationLogs_ShouldReturnInformationLevelLogs()
    {
        // Arrange
        var infoMessage = "Information message";
        _logger.Log(LogLevel.Information, new EventId(1), infoMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Debug, new EventId(2), "Debug message", null, (state, ex) => state.ToString());

        // Act
        var infoLogs = _logger.InformationLogs();

        // Assert
        Assert.AreEqual(1, infoLogs.Count);
        Assert.AreEqual(infoMessage, infoLogs.First());
    }

    [TestMethod]
    public void TraceLogs_ShouldReturnTraceLevelLogs()
    {
        // Arrange
        var traceMessage = "Trace message";
        _logger.Log(LogLevel.Trace, new EventId(1), traceMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Critical, new EventId(2), "Critical message", null, (state, ex) => state.ToString());

        // Act
        var traceLogs = _logger.TraceLogs();

        // Assert
        Assert.AreEqual(1, traceLogs.Count);
        Assert.AreEqual(traceMessage, traceLogs.First());
    }

    [TestMethod]
    public void WarningLogs_ShouldReturnWarningLevelLogs()
    {
        // Arrange
        var warningMessage = "Warning message";
        _logger.Log(LogLevel.Warning, new EventId(1), warningMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Information, new EventId(2), "Info message", null, (state, ex) => state.ToString());

        // Act
        var warningLogs = _logger.WarningLogs();

        // Assert
        Assert.AreEqual(1, warningLogs.Count);
        Assert.AreEqual(warningMessage, warningLogs.First());
    }

    [TestMethod]
    public void ContainsMessage_WithMatchingMessage_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "First message", "Second message", "Third message" };
        var searchFragment = "Second";

        // Act & Assert - Should not throw
        messages.ContainsMessage(searchFragment);
    }

    [TestMethod]
    public void ContainsMessage_WithCaseInsensitiveMatch_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "First Message", "SECOND MESSAGE", "third message" };
        var searchFragment = "second";

        // Act & Assert - Should not throw
        messages.ContainsMessage(searchFragment);
    }

    [TestMethod]
    public void ContainsMessage_WithNonMatchingMessage_ShouldThrowException()
    {
        // Arrange
        var messages = new List<string> { "First message", "Second message", "Third message" };
        var searchFragment = "Fourth";

        // Act & Assert
        var exception = Assert.ThrowsException<Exception>(() => messages.ContainsMessage(searchFragment));
        Assert.IsTrue(exception.Message.Contains(searchFragment));
    }

    [TestMethod]
    public void ContainsMessage_WithEmptyList_ShouldThrowException()
    {
        // Arrange
        var messages = new List<string>();
        var searchFragment = "Any message";

        // Act & Assert
        Assert.ThrowsException<Exception>(() => messages.ContainsMessage(searchFragment));
    }

    [TestMethod]
    public void ContainsMessage_WithPartialMatch_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "This is a complete message" };
        var searchFragment = "complete";

        // Act & Assert - Should not throw
        messages.ContainsMessage(searchFragment);
    }

    [TestMethod]
    public void AssertContains_WithMatchingMessage_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "First message", "Second message", "Third message" };
        var searchFragment = "Second";

        // Act & Assert - Should not throw
        messages.AssertContains(searchFragment);
    }

    [TestMethod]
    public void AssertContains_WithCaseInsensitiveMatch_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "First Message", "SECOND MESSAGE", "third message" };
        var searchFragment = "second";

        // Act & Assert - Should not throw
        messages.AssertContains(searchFragment);
    }

    [TestMethod]
    public void AssertContains_WithNonMatchingMessage_ShouldThrowException()
    {
        // Arrange
        var messages = new List<string> { "First message", "Second message", "Third message" };
        var searchFragment = "Fourth";

        // Act & Assert
        var exception = Assert.ThrowsException<Exception>(() => messages.AssertContains(searchFragment));
        Assert.IsTrue(exception.Message.Contains(searchFragment));
    }

    [TestMethod]
    public void AssertContains_WithEmptyList_ShouldThrowException()
    {
        // Arrange
        var messages = new List<string>();
        var searchFragment = "Any message";

        // Act & Assert
        Assert.ThrowsException<Exception>(() => messages.AssertContains(searchFragment));
    }

    [TestMethod]
    public void AssertContains_WithPartialMatch_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "This is a complete message" };
        var searchFragment = "complete";

        // Act & Assert - Should not throw
        messages.AssertContains(searchFragment);
    }

    private class TestClass { }
}