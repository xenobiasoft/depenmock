using System;
using System.Reflection;
using DepenMock.Attributes;
using DepenMock.Helpers;
using DepenMock.Loggers;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace DepenMock.NUnit.Attributes;

/// <summary>
/// Internal test action attribute that automatically outputs log messages based on LogOutputAttribute configuration.
/// This attribute is automatically applied by the NUnit framework when LogOutputAttribute is present.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
internal class LogOutputTestActionAttribute : TestActionAttribute
{
    private string? _logOutput;

    /// <summary>
    /// Executes before the test to capture initial state.
    /// </summary>
    /// <param name="test">The test that is about to be run.</param>
    public override void BeforeTest(ITest test)
    {
        _logOutput = null;
    }

    /// <summary>
    /// Executes after the test to output logs if conditions are met.
    /// </summary>
    /// <param name="test">The test that was run.</param>
    public override void AfterTest(ITest test)
    {
        try
        {
            var testMethod = test.Method?.MethodInfo;
            var testClass = test.Method?.MethodInfo?.DeclaringType ?? test.Fixture?.GetType();

            if (testMethod == null || testClass == null)
                return;

            // Check if we should output logs
            var testPassed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed;
            
            if (!LogOutputHelper.ShouldOutputLogs(testMethod, testClass, testPassed))
                return;

            // Try to get the logger from the test instance
            var testInstance = test.Fixture;
            if (testInstance == null)
                return;

            var logger = GetLoggerFromTestInstance(testInstance);
            if (logger == null)
                return;

            _logOutput = LogOutputHelper.FormatLogMessages(logger);
            if (!string.IsNullOrWhiteSpace(_logOutput))
            {
                TestContext.WriteLine(_logOutput);
            }
        }
        catch (Exception ex)
        {
            // Don't let log output failures break the test
            TestContext.WriteLine($"Warning: Failed to output log messages - {ex.Message}");
        }
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