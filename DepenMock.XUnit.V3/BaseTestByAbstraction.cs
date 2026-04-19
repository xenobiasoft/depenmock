using DepenMock.Attributes;
using DepenMock.Loggers;
using DepenMock.Mocks;
using Microsoft.Extensions.Logging;
using System;

namespace DepenMock.XUnit.V3;

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
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestByAbstraction"/> class.
    /// </summary>
    /// <remarks>This constructor registers a logger implementation for the specified interface type in the
    /// dependency injection container. The logger is registered as a <see cref="ListLogger{TInterfaceType}"/> instance
    /// associated with the provided <see cref="ILogger{TInterfaceType}"/>.</remarks>
    protected BaseTestByAbstraction(IMockFactory mockFactory) : base(mockFactory)
    {
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
            // Cleanup if needed
        }

        _disposed = true;
    }
}