using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DepenMock.NUnit;

/// <summary>
/// Represents a base class for tests that are specific to a given type.
/// </summary>
/// <remarks>This class provides functionality to resolve the subject under test (SUT) from a dependency injection
/// container. Derived classes can use this functionality to access the SUT.</remarks>
/// <typeparam name="TTestType">The type of the subject under test (SUT). Must be a reference type.</typeparam>
public abstract class BaseTestByType<TTestType> : BaseTest where TTestType : class
{
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
    /// Gets the logger instance used for logging operations specific to the <typeparamref name="TTestType"/> type.
    /// </summary>
    public ListLogger<TTestType> Logger => new ();

    /// <summary>
    /// Sets up the test environment by registering required dependencies in the container.
    /// </summary>
    /// <remarks>This method is executed before each test to ensure the necessary dependencies, such as  the
    /// logger, are properly configured in the container. It is marked with the <see cref="SetUpAttribute"/>  to
    /// indicate that it should run prior to each test in NUnit.</remarks>
    [SetUp]
    public void Setup()
    {
        Container.Register<ILogger<TTestType>>(Logger);
    }
}