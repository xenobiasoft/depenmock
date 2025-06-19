using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DepenMock.NUnit;

/// <summary>
/// Serves as a base class for tests that rely on abstraction, providing functionality to resolve the system under test
/// (SUT).
/// </summary>
/// <remarks>This class is designed to facilitate testing scenarios where the system under test is resolved from a
/// dependency container. Derived classes can use the <see cref="ResolveSut"/> method to retrieve an instance of
/// <typeparamref name="TTestType"/>.</remarks>
/// <typeparam name="TTestType">The concrete type of the system under test. Must be a class that implements <typeparamref name="TInterfaceType"/>.</typeparam>
/// <typeparam name="TInterfaceType">The interface or base type that <typeparamref name="TTestType"/> implements or inherits.</typeparam>
public abstract class BaseTestByAbstraction<TTestType, TInterfaceType> : BaseTest where TTestType : class, TInterfaceType
{
    /// <summary>
    /// Resolves an instance of the system under test (SUT) from the container.
    /// </summary>
    /// <remarks>This method attempts to resolve an instance of the specified type from the container.  If the
    /// container is null or the type cannot be resolved, the method returns <see langword="null"/>.</remarks>
    /// <returns>An instance of <typeparamref name="TInterfaceType"/> if successfully resolved; otherwise, <see
    /// langword="null"/>.</returns>
    protected TInterfaceType? ResolveSut() => Container?.Resolve<TTestType>();

    /// <summary>
    /// Gets the logger instance used for logging messages related to the current test type.
    /// </summary>
    public ListLogger<TTestType> Logger { get; } = new ();

    /// <summary>
    /// Sets up the test environment by registering the logger implementation in the dependency container.
    /// </summary>
    /// <remarks>This method is intended to be used as a setup step in unit tests, ensuring that the specified
    /// logger  implementation is available for dependency injection during the test execution.</remarks>
    [SetUp]
    public void Setup()
    {
        Container.Register<ILogger<TTestType>, ListLogger<TTestType>>(Logger);
        Container.Register<ILogger, ListLogger<TTestType>>(Logger);
    }
}