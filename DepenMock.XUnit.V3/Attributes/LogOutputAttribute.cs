using System;
using System.Reflection;
using DepenMock.Attributes;
using DepenMock.Helpers;
using DepenMock.Loggers;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace DepenMock.XUnit.V3.Attributes;

/// <summary>
/// Attribute that controls when log messages should be output to the test runner's output window in xUnit v3 tests.
/// </summary>
/// <remarks>
/// <para>
/// This attribute combines the functionality of <see cref="DepenMock.Attributes.LogOutputAttribute"/> with xUnit v3's 
/// <see cref="IBeforeAfterTestAttribute"/> to enable automatic log output based on test outcome.
/// Unlike xUnit v2, xUnit v3 provides access to test results, making it possible to conditionally 
/// output logs based on test outcome.
/// </para>
/// <para>
/// Use this attribute instead of <see cref="DepenMock.Attributes.LogOutputAttribute"/> when working with xUnit v3.
/// The standard <see cref="DepenMock.Attributes.LogOutputAttribute"/> will not work with xUnit because xUnit v2 doesn't 
/// provide test outcome information.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class LogOutputAttribute : Attribute, IBeforeAfterTestAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogOutputAttribute"/> class
    /// with the default timing of <see cref="LogOutputTiming.Always"/>.
    /// </summary>
    public LogOutputAttribute() : this(LogOutputTiming.Always)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogOutputAttribute"/> class
    /// with the specified timing.
    /// </summary>
    /// <param name="timing">The timing that determines when log messages should be output.</param>
    public LogOutputAttribute(LogOutputTiming timing)
    {
        Timing = timing;
    }

    /// <summary>
    /// Gets the timing that determines when log messages should be output.
    /// </summary>
    public LogOutputTiming Timing { get; }

    /// <summary>
    /// Executes before the test to capture initial state.
    /// </summary>
    /// <param name="methodUnderTest">The method under test.</param>
    /// <param name="test">The current xUnit test.</param>
    public void Before(MethodInfo methodUnderTest, IXunitTest test)
    {
        // Clear logs from any previous tests to ensure log isolation
        var testContext = TestContext.Current;
        var testInstance = testContext?.TestClassInstance;
        
        if (testInstance != null)
        {
            var logger = GetLoggerFromTestInstance(testInstance);
            logger?.Clear();
        }
    }

    /// <summary>
    /// Executes after the test to output logs if conditions are met.
    /// </summary>
    /// <param name="methodUnderTest">The method under test.</param>
    /// <param name="test">The current xUnit test.</param>
    public void After(MethodInfo methodUnderTest, IXunitTest test)
    {
        try
        {
            var testContext = TestContext.Current;
            if (testContext == null)
                return;

            var testClass = methodUnderTest.DeclaringType;
            if (testClass == null)
                return;

            // Get test result - in xUnit v3, TestState is available after test execution
            var testState = testContext.TestState;
            if (testState == null)
            {
                // If test state is not available yet, assume test passed (this shouldn't happen in After hook)
                return;
            }

            var testPassed = testState.Result == TestResult.Passed;

            // Check if we should output logs based on this attribute's timing configuration
            if (!ShouldOutputForTiming(Timing, testPassed))
                return;

            // Try to get the logger from the test instance
            var testInstance = testContext.TestClassInstance;
            if (testInstance == null)
                return;

            var logger = GetLoggerFromTestInstance(testInstance);
            if (logger == null)
                return;

            var logOutput = LogOutputHelper.FormatLogMessages(logger);
            if (!string.IsNullOrWhiteSpace(logOutput))
            {
                // Use TestContext's output helper to write logs
                var outputHelper = testContext.TestOutputHelper;
                if (outputHelper != null)
                {
                    outputHelper.WriteLine(logOutput);
                }
            }
        }
        catch (Exception ex)
        {
            // Don't let log output failures break the test
            try
            {
                TestContext.Current?.TestOutputHelper?.WriteLine($"Warning: Failed to output log messages - {ex.Message}");
            }
            catch
            {
                // Ignore any errors in error handling
            }
        }
    }

    /// <summary>
    /// Determines if logs should be output based on timing configuration and test result.
    /// </summary>
    /// <param name="timing">The timing configuration.</param>
    /// <param name="testPassed">Whether the test passed.</param>
    /// <returns>True if logs should be output, false otherwise.</returns>
    private static bool ShouldOutputForTiming(LogOutputTiming timing, bool testPassed)
    {
        return timing switch
        {
            LogOutputTiming.Always => true,
            LogOutputTiming.OnFailure => !testPassed,
            LogOutputTiming.OnSuccess => testPassed,
            _ => false
        };
    }

    /// <summary>
    /// Attempts to extract the logger from the test instance using reflection.
    /// </summary>
    /// <param name="testInstance">The test instance.</param>
    /// <returns>The test logger if found, null otherwise.</returns>
    private static ITestLogger? GetLoggerFromTestInstance(object testInstance)
    {
        try
        {
            // Look for a Logger property of type ITestLogger or ListLogger<>
            var properties = testInstance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var prop in properties)
            {
                if (prop.Name == "Logger" && 
                    (typeof(ITestLogger).IsAssignableFrom(prop.PropertyType) ||
                     (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(ListLogger<>))))
                {
                    return prop.GetValue(testInstance) as ITestLogger;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
