using System.Linq;
using DepenMock.Attributes;
using DepenMock.MSTest;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MSTest;

[TestClass]
public class LogOutputAttributeTests : BaseTestByType<TestService>
{
    [TestMethod]
    [LogOutput(LogOutputTiming.Always)]
    public void TestWithLogOutput_Always_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("TestMessage");
        
        // Assert
        Assert.AreEqual(initialLogCount + 1, Logger.Logs[LogLevel.Information].Count);
        Assert.IsTrue(Logger.Logs[LogLevel.Information].Last().Contains("TestMessage"));
    }

    [TestMethod]
    [LogOutput(LogOutputTiming.OnSuccess)]
    public void TestWithLogOutput_OnSuccess_PassingTest_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("SuccessMessage");
        
        // Assert
        Assert.AreEqual(initialLogCount + 1, Logger.Logs[LogLevel.Information].Count);
        Assert.IsTrue(Logger.Logs[LogLevel.Information].Last().Contains("SuccessMessage"));
    }

    [TestMethod]
    [LogOutput(LogOutputTiming.OnFailure)]
    public void TestWithLogOutput_OnFailure_PassingTest_ShouldNotOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("FailureMessage");
        
        // Assert - This test passes, so logs should not be output with OnFailure
        Assert.AreEqual(initialLogCount + 1, Logger.Logs[LogLevel.Information].Count);
    }

    [TestMethod]
    public void TestWithoutLogOutput_ShouldNotOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("NoOutputMessage");
        
        // Assert
        Assert.AreEqual(initialLogCount + 1, Logger.Logs[LogLevel.Information].Count);
    }
}

[TestClass]
[LogOutput(LogOutputTiming.Always)]
public class LogOutputAttributeClassLevelTests : BaseTestByType<TestService>
{
    [TestMethod]
    public void TestWithClassLevelLogOutput_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        var initialLogCount = Logger.Logs[LogLevel.Information].Count;
        
        // Act
        service.DoWork("ClassLevelMessage");
        
        // Assert
        Assert.AreEqual(initialLogCount + 1, Logger.Logs[LogLevel.Information].Count);
        Assert.IsTrue(Logger.Logs[LogLevel.Information].Last().Contains("ClassLevelMessage"));
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