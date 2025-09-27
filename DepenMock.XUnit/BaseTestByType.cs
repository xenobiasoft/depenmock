using System;
using System.Reflection;
using DepenMock.Attributes;
using DepenMock.Helpers;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace DepenMock.XUnit;

/// <summary>
/// Provides a base class for tests that are specific to a given type.
/// </summary>
/// <remarks>This abstract class serves as a foundation for creating tests that are tied to a specific type. It
/// includes functionality for resolving instances of the type from a container and managing logging specific to the
/// type.</remarks>
/// <typeparam name="TTestType">The type associated with the test. This type must be a reference type.</typeparam>
public abstract class BaseTestByType<TTestType> : BaseTest, IDisposable where TTestType : class
{
    private readonly ITestOutputHelper? _outputHelper;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestByType"/> class and registers a logger for the specified
    /// test type.
    /// </summary>
    /// <remarks>This constructor ensures that a logger of type <see cref="ILogger{TTestType}"/> is registered
    /// in the container for the test type. Derived classes can rely on this registration for logging
    /// purposes.</remarks>
    protected BaseTestByType() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestByType"/> class with test output helper support.
    /// </summary>
    /// <param name="outputHelper">The XUnit test output helper for writing test output.</param>
    protected BaseTestByType(ITestOutputHelper? outputHelper)
    {
        _outputHelper = outputHelper;
        Container.Register<ILogger<TTestType>>(Logger);
        AddContainerCustomizations(Container);
    }

    /// <summary>
    /// Allows derived classes to add custom ISpecimenBuilder instances to the container's fixture.
    /// </summary>
    /// <param name="container">The test's dependency injection container.</param>
    protected virtual void AddContainerCustomizations(Container container) { }

    /// <summary>
    /// Resolves an instance of the specified type from the container.
    /// </summary>
    /// <remarks>This method retrieves an instance of <typeparamref name="TTestType"/> from the container, if
    /// available. Ensure that the container is properly initialized and contains a registration for <typeparamref
    /// name="TTestType"/>.</remarks>
    /// <returns>An instance of <typeparamref name="TTestType"/> if the container is not null and the type is registered;
    /// otherwise, <see langword="null"/>.</returns>
    protected TTestType ResolveSut() => Container?.Resolve<TTestType>();

    /// <summary>
    /// Gets a logger instance for recording and managing log entries specific to the type <typeparamref
    /// name="TTestType"/>.
    /// </summary>
    public ListLogger<TTestType> Logger { get; } = new();

    /// <summary>
    /// Outputs log messages if configured and disposes resources.
    /// </summary>
    /// <remarks>This method is called when the test is complete to output log messages when the <see cref="LogOutputAttribute"/>
    /// is present on the test method or class. Since XUnit doesn't provide direct access to test results, it assumes
    /// the test passed if no exception was thrown.</remarks>
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
                // XUnit doesn't provide direct access to test results in dispose, so we assume test passed
                // if we reach disposal without exception
                var testPassed = true;
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