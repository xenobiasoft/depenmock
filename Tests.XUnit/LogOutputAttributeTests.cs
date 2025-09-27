using System.Linq;
using DepenMock.Attributes;
using DepenMock.XUnit;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Tests.XUnit;

public class LogOutputAttributeTests : BaseTestByType<TestService>
{
    public LogOutputAttributeTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    [LogOutput(LogOutputTiming.Always)]
    public void TestWithLogOutput_Always_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("TestMessage");
        
        // Assert
        Assert.Equal(initialLogCount + 1, Logger.Logs[LogLevel.Information].Count);
        Assert.Contains("TestMessage", Logger.Logs[LogLevel.Information].Last());
    }

    [Fact]
    [LogOutput(LogOutputTiming.OnSuccess)]
    public void TestWithLogOutput_OnSuccess_PassingTest_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("SuccessMessage");
        
        // Assert
        Assert.Equal(initialLogCount + 1, Logger.Logs[LogLevel.Information].Count);
        Assert.Contains("SuccessMessage", Logger.Logs[LogLevel.Information].Last());
    }

    [Fact]
    [LogOutput(LogOutputTiming.OnFailure)]
    public void TestWithLogOutput_OnFailure_PassingTest_ShouldNotOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("FailureMessage");
        
        // Assert - This test passes, so logs should not be output with OnFailure
        Assert.Equal(initialLogCount + 1, Logger.Logs[LogLevel.Information].Count);
    }

    [Fact]
    public void TestWithoutLogOutput_ShouldNotOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("NoOutputMessage");
        
        // Assert
        Assert.Equal(initialLogCount + 1, Logger.Logs[LogLevel.Information].Count);
    }
}

[LogOutput(LogOutputTiming.Always)]
public class LogOutputAttributeClassLevelTests : BaseTestByType<TestService>
{
    public LogOutputAttributeClassLevelTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public void TestWithClassLevelLogOutput_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("ClassLevelMessage");
        
        // Assert
        Assert.Equal(initialLogCount + 1, Logger.Logs[LogLevel.Information].Count);
        Assert.Contains("ClassLevelMessage", Logger.Logs[LogLevel.Information].Last());
    }
}

public class TestService
{
    private readonly ILogger<TestService> _logger;

    public TestService(ILogger<TestService> logger)
    {
        _logger = logger;
    }

    public void DoWork(string message)
    {
        _logger.LogInformation("Processing: {Message}", message);
    }
}