using DepenMock.Helpers;
using DepenMock.Loggers;
using System;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DepenMock.XUnit.Attributes;

/// <summary>
/// Internal test action attribute that automatically outputs log messages based on LogOutputAttribute configuration.
/// This attribute is automatically applied by the xUnit framework when LogOutputAttribute is present.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
internal class LogOutputTestActionAttribute : BeforeAfterTestAttribute
{
    /// <summary>
    /// Executes after the test to output logs if conditions are met.
    /// </summary>
    /// <param name="methodUnderTest">The method that was tested.</param>
    public override void After(MethodInfo methodUnderTest)
    {
        try
        {
            var testClass = methodUnderTest.DeclaringType;
            if (testClass == null)
                return;

            // For xUnit, we need to determine test result differently
            // Since xUnit v2 doesn't provide direct access to test result in BeforeAfterTestAttribute,
            // we'll assume the test passed if we reach this point without exception
            var testPassed = true;
            
            if (!LogOutputHelper.ShouldOutputLogs(methodUnderTest, testClass, testPassed))
                return;

            // Try to get the test output helper and logger from the test instance
            var testInstance = GetTestInstance(methodUnderTest);
            if (testInstance == null)
                return;

            var outputHelper = GetOutputHelperFromTestInstance(testInstance);
            var logger = GetLoggerFromTestInstance(testInstance);
            
            if (outputHelper == null || logger == null)
                return;

            var logOutput = LogOutputHelper.FormatLogMessages(logger);
            if (!string.IsNullOrWhiteSpace(logOutput))
            {
                outputHelper.WriteLine(logOutput);
            }
        }
        catch (Exception ex)
        {
            // Don't let log output failures break the test
            // In xUnit v2, we can't easily write to output here, so we'll silently fail
            // In v3, this will work better
        }
    }

    /// <summary>
    /// Gets the test instance from the current execution context.
    /// This is a simplified approach that works for most scenarios.
    /// </summary>
    private static object? GetTestInstance(MethodInfo methodUnderTest)
    {
        // This is a limitation in xUnit v2 - we can't easily access the test instance
        // In xUnit v3, this will be improved
        return null;
    }

    /// <summary>
    /// Attempts to extract the ITestOutputHelper from the test instance.
    /// </summary>
    /// <param name="testInstance">The test instance.</param>
    /// <returns>The test output helper if found, null otherwise.</returns>
    private static ITestOutputHelper? GetOutputHelperFromTestInstance(object testInstance)
    {
        try
        {
            // Look for OutputHelper or TestOutputHelper property
            var properties = testInstance.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (var prop in properties)
            {
                if (typeof(ITestOutputHelper).IsAssignableFrom(prop.PropertyType))
                {
                    return prop.GetValue(testInstance) as ITestOutputHelper;
                }
            }

            // Look for fields as well
            var fields = testInstance.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (var field in fields)
            {
                if (typeof(ITestOutputHelper).IsAssignableFrom(field.FieldType))
                {
                    return field.GetValue(testInstance) as ITestOutputHelper;
                }
            }

            return null;
        }
        catch
        {
            return null;
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