using DepenMock.Attributes;
using DepenMock.Helpers;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MSTest;

[TestClass]
public class LogOutputHelperTests
{
    [TestMethod]
    public void FormatLogMessages_WithNullLogger_ShouldReturnNull()
    {
        // Act
        var result = LogOutputHelper.FormatLogMessages(null);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FormatLogMessages_WithEmptyLogs_ShouldReturnNull()
    {
        // Arrange
        var logger = new ListLogger<TestClass>();

        // Act
        var result = LogOutputHelper.FormatLogMessages(logger);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void FormatLogMessages_WithLogMessages_ShouldReturnFormattedString()
    {
        // Arrange
        var logger = new ListLogger<TestClass>();
        logger.Log(LogLevel.Information, new EventId(1), "Info message", null, (state, ex) => state.ToString());
        logger.Log(LogLevel.Error, new EventId(2), "Error message", null, (state, ex) => state.ToString());

        // Act
        var result = LogOutputHelper.FormatLogMessages(logger);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Contains("=== Test Log Messages ==="));
        Assert.IsTrue(result.Contains("[Information]"));
        Assert.IsTrue(result.Contains("Info message"));
        Assert.IsTrue(result.Contains("[Error]"));
        Assert.IsTrue(result.Contains("Error message"));
    }

    [TestMethod]
    public void ShouldOutputLogs_WithMethodLevelAlwaysAttribute_ShouldReturnTrue()
    {
        // Arrange
        var method = typeof(TestClassWithMethodAttribute).GetMethod(nameof(TestClassWithMethodAttribute.TestMethodWithAlways));
        var testClass = typeof(TestClassWithMethodAttribute);

        // Act & Assert
        Assert.IsTrue(LogOutputHelper.ShouldOutputLogs(method, testClass, true));
        Assert.IsTrue(LogOutputHelper.ShouldOutputLogs(method, testClass, false));
    }

    [TestMethod]
    public void ShouldOutputLogs_WithMethodLevelOnSuccessAttribute_ShouldReturnTrueOnlyWhenTestPasses()
    {
        // Arrange
        var method = typeof(TestClassWithMethodAttribute).GetMethod(nameof(TestClassWithMethodAttribute.TestMethodWithOnSuccess));
        var testClass = typeof(TestClassWithMethodAttribute);

        // Act & Assert
        Assert.IsTrue(LogOutputHelper.ShouldOutputLogs(method, testClass, true));
        Assert.IsFalse(LogOutputHelper.ShouldOutputLogs(method, testClass, false));
    }

    [TestMethod]
    public void ShouldOutputLogs_WithMethodLevelOnFailureAttribute_ShouldReturnTrueOnlyWhenTestFails()
    {
        // Arrange
        var method = typeof(TestClassWithMethodAttribute).GetMethod(nameof(TestClassWithMethodAttribute.TestMethodWithOnFailure));
        var testClass = typeof(TestClassWithMethodAttribute);

        // Act & Assert
        Assert.IsFalse(LogOutputHelper.ShouldOutputLogs(method, testClass, true));
        Assert.IsTrue(LogOutputHelper.ShouldOutputLogs(method, testClass, false));
    }

    [TestMethod]
    public void ShouldOutputLogs_WithClassLevelAttribute_ShouldRespectClassAttribute()
    {
        // Arrange
        var method = typeof(TestClassWithClassAttribute).GetMethod(nameof(TestClassWithClassAttribute.TestMethodWithoutAttribute));
        var testClass = typeof(TestClassWithClassAttribute);

        // Act & Assert
        Assert.IsTrue(LogOutputHelper.ShouldOutputLogs(method, testClass, true));
        Assert.IsTrue(LogOutputHelper.ShouldOutputLogs(method, testClass, false));
    }

    [TestMethod]
    public void ShouldOutputLogs_WithMethodAttributeOverridingClassAttribute_ShouldPrioritizeMethodAttribute()
    {
        // Arrange
        var method = typeof(TestClassWithClassAttribute).GetMethod(nameof(TestClassWithClassAttribute.TestMethodWithOnFailureOverride));
        var testClass = typeof(TestClassWithClassAttribute);

        // Act & Assert - Method has OnFailure, class has Always, method should win
        Assert.IsFalse(LogOutputHelper.ShouldOutputLogs(method, testClass, true));
        Assert.IsTrue(LogOutputHelper.ShouldOutputLogs(method, testClass, false));
    }

    [TestMethod]
    public void ShouldOutputLogs_WithNoAttributes_ShouldReturnFalse()
    {
        // Arrange
        var method = typeof(TestClassWithoutAttributes).GetMethod(nameof(TestClassWithoutAttributes.TestMethod));
        var testClass = typeof(TestClassWithoutAttributes);

        // Act & Assert
        Assert.IsFalse(LogOutputHelper.ShouldOutputLogs(method, testClass, true));
        Assert.IsFalse(LogOutputHelper.ShouldOutputLogs(method, testClass, false));
    }

    [TestMethod]
    public void ShouldOutputLogs_WithNullMethod_ShouldReturnFalse()
    {
        // Arrange
        var testClass = typeof(TestClassWithoutAttributes);

        // Act & Assert
        Assert.IsFalse(LogOutputHelper.ShouldOutputLogs(null, testClass, true));
        Assert.IsFalse(LogOutputHelper.ShouldOutputLogs(null, testClass, false));
    }

    [TestMethod]
    public void ShouldOutputLogs_WithNullClass_ShouldReturnFalse()
    {
        // Arrange
        var method = typeof(TestClassWithoutAttributes).GetMethod(nameof(TestClassWithoutAttributes.TestMethod));

        // Act & Assert
        Assert.IsFalse(LogOutputHelper.ShouldOutputLogs(method, null, true));
        Assert.IsFalse(LogOutputHelper.ShouldOutputLogs(method, null, false));
    }

    private class TestClass { }

    private class TestClassWithMethodAttribute
    {
        [LogOutput(LogOutputTiming.Always)]
        public void TestMethodWithAlways() { }

        [LogOutput(LogOutputTiming.OnSuccess)]
        public void TestMethodWithOnSuccess() { }

        [LogOutput(LogOutputTiming.OnFailure)]
        public void TestMethodWithOnFailure() { }
    }

    [LogOutput(LogOutputTiming.Always)]
    private class TestClassWithClassAttribute
    {
        public void TestMethodWithoutAttribute() { }

        [LogOutput(LogOutputTiming.OnFailure)]
        public void TestMethodWithOnFailureOverride() { }
    }

    private class TestClassWithoutAttributes
    {
        public void TestMethod() { }
    }
}