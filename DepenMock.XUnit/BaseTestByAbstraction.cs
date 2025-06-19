using DepenMock.Loggers;
using Microsoft.Extensions.Logging;

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
public abstract class BaseTestByAbstraction<TTestType, TInterfaceType> : BaseTest where TTestType : class, TInterfaceType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestByAbstraction"/> class.
    /// </summary>
    /// <remarks>This constructor registers a logger implementation for the specified interface type in the
    /// dependency injection container. The logger is registered as a <see cref="ListLogger{TInterfaceType}"/> instance
    /// associated with the provided <see cref="ILogger{TInterfaceType}"/>.</remarks>
    protected BaseTestByAbstraction()
    {
        Container.Register<ILogger<TTestType>, ListLogger<TTestType>>(Logger);
        Container.Register<ILogger, ListLogger<TTestType>>(Logger);
    }

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
}