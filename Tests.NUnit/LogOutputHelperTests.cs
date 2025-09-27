using DepenMock.Attributes;
using DepenMock.Helpers;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests.NUnit;

[TestFixture]
public class LogOutputHelperTests
{
    [Test]
    public void FormatLogMessages_WithNullLogger_ShouldReturnNull()
    {
        // Act
        var result = LogOutputHelper.FormatLogMessages(null);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void FormatLogMessages_WithEmptyLogs_ShouldReturnNull()
    {
        // Arrange
        var logger = new ListLogger<TestClass>();

        // Act
        var result = LogOutputHelper.FormatLogMessages(logger);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void FormatLogMessages_WithLogMessages_ShouldReturnFormattedString()
    {
        // Arrange
        var logger = new ListLogger<TestClass>();
        logger.Log(LogLevel.Information, new EventId(1), "Info message", null, (state, ex) => state.ToString());
        logger.Log(LogLevel.Error, new EventId(2), "Error message", null, (state, ex) => state.ToString());

        // Act
        var result = LogOutputHelper.FormatLogMessages(logger);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Does.Contain("=== Test Log Messages ==="));
        Assert.That(result, Does.Contain("[Information]"));
        Assert.That(result, Does.Contain("Info message"));
        Assert.That(result, Does.Contain("[Error]"));
        Assert.That(result, Does.Contain("Error message"));
    }

    [Test]
    public void ShouldOutputLogs_WithMethodLevelAlwaysAttribute_ShouldReturnTrue()
    {
        // Arrange
        var method = typeof(TestClassWithMethodAttribute).GetMethod(nameof(TestClassWithMethodAttribute.TestMethodWithAlways));
        var testClass = typeof(TestClassWithMethodAttribute);

        // Act & Assert
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, true), Is.True);
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, false), Is.True);
    }

    [Test]
    public void ShouldOutputLogs_WithMethodLevelOnSuccessAttribute_ShouldReturnTrueOnlyWhenTestPasses()
    {
        // Arrange
        var method = typeof(TestClassWithMethodAttribute).GetMethod(nameof(TestClassWithMethodAttribute.TestMethodWithOnSuccess));
        var testClass = typeof(TestClassWithMethodAttribute);

        // Act & Assert
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, true), Is.True);
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, false), Is.False);
    }

    [Test]
    public void ShouldOutputLogs_WithMethodLevelOnFailureAttribute_ShouldReturnTrueOnlyWhenTestFails()
    {
        // Arrange
        var method = typeof(TestClassWithMethodAttribute).GetMethod(nameof(TestClassWithMethodAttribute.TestMethodWithOnFailure));
        var testClass = typeof(TestClassWithMethodAttribute);

        // Act & Assert
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, true), Is.False);
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, false), Is.True);
    }

    [Test]
    public void ShouldOutputLogs_WithClassLevelAttribute_ShouldRespectClassAttribute()
    {
        // Arrange
        var method = typeof(TestClassWithClassAttribute).GetMethod(nameof(TestClassWithClassAttribute.TestMethodWithoutAttribute));
        var testClass = typeof(TestClassWithClassAttribute);

        // Act & Assert
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, true), Is.True);
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, false), Is.True);
    }

    [Test]
    public void ShouldOutputLogs_WithMethodAttributeOverridingClassAttribute_ShouldPrioritizeMethodAttribute()
    {
        // Arrange
        var method = typeof(TestClassWithClassAttribute).GetMethod(nameof(TestClassWithClassAttribute.TestMethodWithOnFailureOverride));
        var testClass = typeof(TestClassWithClassAttribute);

        // Act & Assert - Method has OnFailure, class has Always, method should win
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, true), Is.False);
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, false), Is.True);
    }

    [Test]
    public void ShouldOutputLogs_WithNoAttributes_ShouldReturnFalse()
    {
        // Arrange
        var method = typeof(TestClassWithoutAttributes).GetMethod(nameof(TestClassWithoutAttributes.TestMethod));
        var testClass = typeof(TestClassWithoutAttributes);

        // Act & Assert
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, true), Is.False);
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, testClass, false), Is.False);
    }

    [Test]
    public void ShouldOutputLogs_WithNullMethod_ShouldReturnFalse()
    {
        // Arrange
        var testClass = typeof(TestClassWithoutAttributes);

        // Act & Assert
        Assert.That(LogOutputHelper.ShouldOutputLogs(null, testClass, true), Is.False);
        Assert.That(LogOutputHelper.ShouldOutputLogs(null, testClass, false), Is.False);
    }

    [Test]
    public void ShouldOutputLogs_WithNullClass_ShouldReturnFalse()
    {
        // Arrange
        var method = typeof(TestClassWithoutAttributes).GetMethod(nameof(TestClassWithoutAttributes.TestMethod));

        // Act & Assert
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, null, true), Is.False);
        Assert.That(LogOutputHelper.ShouldOutputLogs(method, null, false), Is.False);
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