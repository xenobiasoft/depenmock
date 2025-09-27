using DepenMock;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Tests.XUnit;

public class ListLoggerAssertionExtensionsTests
{
    private readonly ListLogger<TestClass> _logger;

    public ListLoggerAssertionExtensionsTests()
    {
        _logger = new ListLogger<TestClass>();
    }

    [Fact]
    public void CriticalLogs_ShouldReturnCriticalLevelLogs()
    {
        // Arrange
        var criticalMessage = "Critical error occurred";
        _logger.Log(LogLevel.Critical, new EventId(1), criticalMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Information, new EventId(2), "Info message", null, (state, ex) => state.ToString());

        // Act
        var criticalLogs = _logger.CriticalLogs();

        // Assert
        Assert.Equal(1, criticalLogs.Count);
        Assert.Equal(criticalMessage, criticalLogs.First());
    }

    [Fact]
    public void DebugLogs_ShouldReturnDebugLevelLogs()
    {
        // Arrange
        var debugMessage = "Debug information";
        _logger.Log(LogLevel.Debug, new EventId(1), debugMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Error, new EventId(2), "Error message", null, (state, ex) => state.ToString());

        // Act
        var debugLogs = _logger.DebugLogs();

        // Assert
        Assert.Equal(1, debugLogs.Count);
        Assert.Equal(debugMessage, debugLogs.First());
    }

    [Fact]
    public void ErrorLogs_ShouldReturnErrorLevelLogs()
    {
        // Arrange
        var errorMessage = "An error occurred";
        _logger.Log(LogLevel.Error, new EventId(1), errorMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Warning, new EventId(2), "Warning message", null, (state, ex) => state.ToString());

        // Act
        var errorLogs = _logger.ErrorLogs();

        // Assert
        Assert.Equal(1, errorLogs.Count);
        Assert.Equal(errorMessage, errorLogs.First());
    }

    [Fact]
    public void InformationLogs_ShouldReturnInformationLevelLogs()
    {
        // Arrange
        var infoMessage = "Information message";
        _logger.Log(LogLevel.Information, new EventId(1), infoMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Debug, new EventId(2), "Debug message", null, (state, ex) => state.ToString());

        // Act
        var infoLogs = _logger.InformationLogs();

        // Assert
        Assert.Equal(1, infoLogs.Count);
        Assert.Equal(infoMessage, infoLogs.First());
    }

    [Fact]
    public void TraceLogs_ShouldReturnTraceLevelLogs()
    {
        // Arrange
        var traceMessage = "Trace message";
        _logger.Log(LogLevel.Trace, new EventId(1), traceMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Critical, new EventId(2), "Critical message", null, (state, ex) => state.ToString());

        // Act
        var traceLogs = _logger.TraceLogs();

        // Assert
        Assert.Equal(1, traceLogs.Count);
        Assert.Equal(traceMessage, traceLogs.First());
    }

    [Fact]
    public void WarningLogs_ShouldReturnWarningLevelLogs()
    {
        // Arrange
        var warningMessage = "Warning message";
        _logger.Log(LogLevel.Warning, new EventId(1), warningMessage, null, (state, ex) => state.ToString());
        _logger.Log(LogLevel.Information, new EventId(2), "Info message", null, (state, ex) => state.ToString());

        // Act
        var warningLogs = _logger.WarningLogs();

        // Assert
        Assert.Equal(1, warningLogs.Count);
        Assert.Equal(warningMessage, warningLogs.First());
    }

    [Fact]
    public void ContainsMessage_WithMatchingMessage_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "First message", "Second message", "Third message" };
        var searchFragment = "Second";

        // Act
        var exception = Record.Exception(() => messages.ContainsMessage(searchFragment));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ContainsMessage_WithCaseInsensitiveMatch_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "First Message", "SECOND MESSAGE", "third message" };
        var searchFragment = "second";

        // Act
        var exception = Record.Exception(() => messages.ContainsMessage(searchFragment));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ContainsMessage_WithNonMatchingMessage_ShouldThrowException()
    {
        // Arrange
        var messages = new List<string> { "First message", "Second message", "Third message" };
        var searchFragment = "Fourth";

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => messages.ContainsMessage(searchFragment));
        Assert.Contains(searchFragment, exception.Message);
    }

    [Fact]
    public void ContainsMessage_WithEmptyList_ShouldThrowException()
    {
        // Arrange
        var messages = new List<string>();
        var searchFragment = "Any message";

        // Act & Assert
        Assert.Throws<Exception>(() => messages.ContainsMessage(searchFragment));
    }

    [Fact]
    public void ContainsMessage_WithPartialMatch_ShouldNotThrow()
    {
        // Arrange
        var messages = new List<string> { "This is a complete message" };
        var searchFragment = "complete";

        // Act
        var exception = Record.Exception(() => messages.ContainsMessage(searchFragment));

        // Assert
        Assert.Null(exception);
    }

    private class TestClass { }
}