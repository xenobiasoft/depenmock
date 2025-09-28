using System;
using System.Reflection;

namespace DepenMock.XUnit.Internal;

/// <summary>
/// Helper class to detect and interact with xUnit v3 features when available.
/// </summary>
/// <remarks>
/// This class uses reflection to detect if xUnit v3 is available and provides
/// access to test context and test outcome information. For xUnit v2, it falls
/// back to assuming the test passed if no exception was thrown.
/// </remarks>
public static class XUnitV3Helper
{
    private static readonly Lazy<bool> _isXUnitV3Available = new(DetectXUnitV3);
    private static readonly Lazy<Type?> _testContextAccessorType = new(GetTestContextAccessorType);
    private static readonly Lazy<Type?> _testContextType = new(GetTestContextType);
    private static readonly Lazy<Type?> _testResultStateType = new(GetTestResultStateType);
    
    /// <summary>
    /// Gets a value indicating whether xUnit v3 is available.
    /// </summary>
    public static bool IsXUnitV3Available => _isXUnitV3Available.Value;
    
    /// <summary>
    /// Attempts to get the current test result state from xUnit v3 context.
    /// </summary>
    /// <returns>
    /// A tuple containing (isAvailable, testPassed) where:
    /// - isAvailable: true if xUnit v3 context is available and test state could be determined
    /// - testPassed: true if the test passed, false if it failed
    /// </returns>
    public static (bool IsAvailable, bool TestPassed) TryGetTestResult()
    {
        if (!IsXUnitV3Available)
        {
            return (false, false);
        }
        
        try
        {
            // Get the current test context using TestContextAccessor.Current
            var testContextAccessorType = _testContextAccessorType.Value;
            if (testContextAccessorType == null)
                return (false, false);
                
            var currentProperty = testContextAccessorType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
            if (currentProperty == null)
                return (false, false);
                
            var testContext = currentProperty.GetValue(null);
            if (testContext == null)
                return (false, false);
            
            // Get the TestState property from the context
            var testStateProperty = testContext.GetType().GetProperty("TestState");
            if (testStateProperty == null)
                return (false, false);
                
            var testState = testStateProperty.GetValue(testContext);
            if (testState == null)
                return (false, false);
            
            // Check if the test state indicates success
            // In xUnit v3, TestResultState has values like: Passed, Failed, Skipped, etc.
            var testStateName = testState.ToString();
            var testPassed = string.Equals(testStateName, "Passed", StringComparison.OrdinalIgnoreCase);
            
            return (true, testPassed);
        }
        catch
        {
            // If anything goes wrong with reflection, fall back to unavailable
            return (false, false);
        }
    }
    
    private static bool DetectXUnitV3()
    {
        try
        {
            // Try to load a type that's only available in xUnit v3
            var assembly = Assembly.Load("xunit.v3.core");
            return assembly != null && assembly.GetType("Xunit.TestContextAccessor") != null;
        }
        catch
        {
            return false;
        }
    }
    
    private static Type? GetTestContextAccessorType()
    {
        try
        {
            var assembly = Assembly.Load("xunit.v3.core");
            return assembly?.GetType("Xunit.TestContextAccessor");
        }
        catch
        {
            return null;
        }
    }
    
    private static Type? GetTestContextType()
    {
        try
        {
            var assembly = Assembly.Load("xunit.v3.core");
            return assembly?.GetType("Xunit.ITestContext");
        }
        catch
        {
            return null;
        }
    }
    
    private static Type? GetTestResultStateType()
    {
        try
        {
            var assembly = Assembly.Load("xunit.v3.core");
            return assembly?.GetType("Xunit.TestResultState");
        }
        catch
        {
            return null;
        }
    }
}