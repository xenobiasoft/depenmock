using System.Linq;
using DepenMock.Attributes;
using DepenMock.NUnit;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests.NUnit;

[TestFixture]
public class LogOutputAttributeTests : BaseTestByType<TestService>
{
    [Test]
    [LogOutput(LogOutputTiming.Always)]
    public void TestWithLogOutput_Always_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("TestMessage");
        
        // Assert
        Assert.That(Logger.Logs[LogLevel.Information], Has.Count.EqualTo(initialLogCount + 1));
        Assert.That(Logger.Logs[LogLevel.Information].Last(), Does.Contain("TestMessage"));
    }

    [Test]
    [LogOutput(LogOutputTiming.OnSuccess)]
    public void TestWithLogOutput_OnSuccess_PassingTest_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("SuccessMessage");
        
        // Assert
        Assert.That(Logger.Logs[LogLevel.Information], Has.Count.EqualTo(initialLogCount + 1));
        Assert.That(Logger.Logs[LogLevel.Information].Last(), Does.Contain("SuccessMessage"));
    }

    [Test]
    [LogOutput(LogOutputTiming.OnFailure)]
    public void TestWithLogOutput_OnFailure_PassingTest_ShouldNotOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("FailureMessage");
        
        // Assert - This test passes, so logs should not be output with OnFailure
        Assert.That(Logger.Logs[LogLevel.Information], Has.Count.EqualTo(initialLogCount + 1));
    }

    [Test]
    public void TestWithoutLogOutput_ShouldNotOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("NoOutputMessage");
        
        // Assert
        Assert.That(Logger.Logs[LogLevel.Information], Has.Count.EqualTo(initialLogCount + 1));
    }
}

[TestFixture]
[LogOutput(LogOutputTiming.Always)]
public class LogOutputAttributeClassLevelTests : BaseTestByType<TestService>
{
    [Test]
    public void TestWithClassLevelLogOutput_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("ClassLevelMessage");
        
        // Assert
        Assert.That(Logger.Logs[LogLevel.Information], Has.Count.EqualTo(initialLogCount + 1));
        Assert.That(Logger.Logs[LogLevel.Information].Last(), Does.Contain("ClassLevelMessage"));
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