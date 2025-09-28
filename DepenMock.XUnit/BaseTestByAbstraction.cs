using System;
using System.Reflection;
using DepenMock.Attributes;
using DepenMock.Helpers;
using DepenMock.Loggers;
using DepenMock.XUnit.Internal;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace DepenMock.XUnit;

/// <summary>
/// Provides a base class for test implementations that rely on abstraction, enabling the resolution  of a system under
/// test (SUT) and logging functionality.
/// </summary>
/// <remarks>This class is designed to facilitate testing scenarios where the system under test is resolved  from
/// a dependency injection container and logging is required. Ensure that the dependency injection  container is
/// properly configured with the necessary registrations for <typeparamref name="TTestType"/>  before using this
/// class.</remarks>
/// <typeparam name="TTestType">The concrete type of the system under test, which must implement <typeparamref name="TInterfaceType"/>.</typeparam>
/// <typeparam name="TInterfaceType">The interface type that the system under test implements.</typeparam>
public abstract class BaseTestByAbstraction<TTestType, TInterfaceType> : BaseTest, IDisposable where TTestType : class, TInterfaceType
{
    private readonly ITestOutputHelper? _outputHelper;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestByAbstraction"/> class.
    /// </summary>
    /// <remarks>This constructor registers a logger implementation for the specified interface type in the
    /// dependency injection container. The logger is registered as a <see cref="ListLogger{TInterfaceType}"/> instance
    /// associated with the provided <see cref="ILogger{TInterfaceType}"/>.</remarks>
    protected BaseTestByAbstraction() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestByAbstraction"/> class with test output helper support.
    /// </summary>
    /// <param name="outputHelper">The XUnit test output helper for writing test output.</param>
    protected BaseTestByAbstraction(ITestOutputHelper? outputHelper)
    {
        _outputHelper = outputHelper;
        Logger.Clear(); // Clear any previous logs (defensive programming)
        Container.Register<ILogger<TTestType>, ListLogger<TTestType>>(Logger);
        Container.Register<ILogger, ListLogger<TTestType>>(Logger);
        AddContainerCustomizations(Container);
    }

    /// <summary>
    /// Allows derived classes to add custom ISpecimenBuilder instances to the container's fixture.
    /// </summary>
    /// <param name="container">The test's dependency injection container.</param>
    protected virtual void AddContainerCustomizations(Container container) { }

    /// <summary>
    /// Resolves an instance of the system under test (SUT) from the dependency injection container.
    /// </summary>
    /// <remarks>Ensure that the dependency injection container is properly configured and contains  a
    /// registration for <typeparamref name="TTestType"/> before calling this method.</remarks>
    /// <returns>An instance of <typeparamref name="TInterfaceType"/> resolved from the container,  or <see langword="null"/> if
    /// the container is not available.</returns>
    protected TInterfaceType ResolveSut() => Container?.Resolve<TTestType>();

    /// <summary>
    /// Gets a logger instance for the specified interface type.
    /// </summary>
    public ListLogger<TTestType> Logger { get; } = new();

    /// <summary>
    /// Outputs log messages if configured and disposes resources.
    /// </summary>
    /// <remarks>This method is called when the test is complete to output log messages when the <see cref="LogOutputAttribute"/>
    /// is present on the test method or class. For xUnit v3, it uses the test context to determine the actual test result.
    /// For xUnit v2, it assumes the test passed if no exception was thrown during disposal.</remarks>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the instance and optionally outputs log messages.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            try
            {
                // Try to get actual test result from xUnit v3 context
                var (isXunitV3Available, testPassed) = XUnitV3Helper.TryGetTestResult();
                
                if (!isXunitV3Available)
                {
                    // Fall back to xUnit v2 behavior: assume test passed if we reach disposal without exception
                    testPassed = true;
                }
                
                var testMethod = GetCurrentTestMethod();
                var testClass = GetType();

                if (testMethod != null && LogOutputHelper.ShouldOutputLogs(testMethod, testClass, testPassed))
                {
                    var logOutput = LogOutputHelper.FormatLogMessages(Logger);
                    if (!string.IsNullOrWhiteSpace(logOutput) && _outputHelper != null)
                    {
                        _outputHelper.WriteLine(logOutput);
                    }
                }
            }
            catch (Exception ex)
            {
                // Don't let log output failures break the test
                _outputHelper?.WriteLine($"Warning: Failed to output log messages - {ex.Message}");
            }
        }

        _disposed = true;
    }

    /// <summary>
    /// Gets the current test method using reflection and stack trace analysis.
    /// </summary>
    /// <returns>The current test method or null if not found.</returns>
    private MethodInfo? GetCurrentTestMethod()
    {
        try
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            var frames = stackTrace.GetFrames();

            foreach (var frame in frames)
            {
                var method = frame.GetMethod();
                if (method != null && 
                    method.DeclaringType != null && 
                    method.DeclaringType.IsAssignableFrom(GetType()) &&
                    method.GetCustomAttribute<FactAttribute>() != null)
                {
                    return method as MethodInfo;
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