using DepenMock.Attributes;
using DepenMock.XUnit;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Tests.XUnit;

/// <summary>
/// Tests for LogOutput attribute behavior with failing tests.
/// These tests validate that LogOutput works correctly based on test outcomes.
/// </summary>
public class LogOutputFailureTests : BaseTestByType<TestService>
{
    public LogOutputFailureTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact(Skip = "Intentionally failing test for manual validation of LogOutput behavior with failures")]
    [LogOutput(LogOutputTiming.OnFailure)]
    public void TestWithLogOutput_OnFailure_FailingTest_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        
        // Act - Log a message that should appear when test fails
        service.DoWork("This message should appear on failure");
        
        // Assert - Force test failure
        Assert.True(false, "Intentionally failing test to validate OnFailure log output");
    }

    [Fact(Skip = "Intentionally failing test for manual validation of LogOutput behavior with failures")]
    [LogOutput(LogOutputTiming.OnSuccess)]
    public void TestWithLogOutput_OnSuccess_FailingTest_ShouldNotOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        
        // Act - Log a message that should NOT appear when test fails
        service.DoWork("This message should NOT appear when test fails");
        
        // Assert - Force test failure
        Assert.True(false, "Intentionally failing test to validate OnSuccess does not output on failure");
    }

    [Fact(Skip = "Intentionally failing test for manual validation of LogOutput behavior with failures")]
    [LogOutput(LogOutputTiming.Always)]
    public void TestWithLogOutput_Always_FailingTest_ShouldOutputLogs()
    {
        // Arrange
        var service = ResolveSut();
        
        // Act - Log a message that should always appear
        service.DoWork("This message should always appear");
        
        // Assert - Force test failure
        Assert.True(false, "Intentionally failing test to validate Always outputs on failure");
    }
}