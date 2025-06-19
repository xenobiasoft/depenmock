using DepenMock.Loggers;
using Microsoft.Extensions.Logging;

namespace DepenMock.MSTest;

/// <summary>
/// Provides a base class for test implementations that utilize abstraction, enabling dependency injection and logging
/// functionality for the system under test (SUT).
/// </summary>
/// <remarks>This class serves as a foundation for tests that require resolving the SUT from a dependency
/// injection container and logging operations. It ensures that the SUT is properly constructed with its dependencies
/// and provides a logger instance for test-related logging.</remarks>
/// <typeparam name="TTestType">The concrete type of the system under test (SUT).</typeparam>
/// <typeparam name="TInterfaceType">The interface type implemented by <typeparamref name="TTestType"/>.</typeparam>
public abstract class BaseTestByAbstraction<TTestType, TInterfaceType> : BaseTest where TTestType : class, TInterfaceType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestByAbstraction"/> class.
    /// </summary>
    /// <remarks>This constructor registers a logger implementation for the specified interface type in the
    /// dependency injection container. The logger is configured using the provided <see cref="Logger"/>
    /// instance.</remarks>
    protected BaseTestByAbstraction()
    {
        Container.Register<ILogger<TTestType>, ListLogger<TTestType>>(Logger);
        Container.Register<ILogger, ListLogger<TTestType>>(Logger);
    }

    /// <summary>
    /// Resolves an instance of the system under test (SUT) from the dependency injection container.
    /// </summary>
    /// <remarks>This method retrieves an instance of the specified type <typeparamref
    /// name="TTestType"/> from the container,  ensuring that the SUT is properly constructed with its dependencies.
    /// If the container is null, the method  returns null.</remarks>
    /// <returns>An instance of <typeparamref name="TTestType"/> if the container is available; otherwise, null.</returns>
    protected TInterfaceType ResolveSut() => Container?.Resolve<TTestType>();

    /// <summary>
    /// Gets the logger instance used for logging operations.
    /// </summary>
    public ListLogger<TTestType> Logger { get; } = new();
}