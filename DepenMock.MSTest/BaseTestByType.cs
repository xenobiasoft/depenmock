using DepenMock.Loggers;
using Microsoft.Extensions.Logging;

namespace DepenMock.MSTest;

/// <summary>
/// Represents a base class for tests that are specific to a given type.
/// </summary>
/// <remarks>This class provides functionality to resolve the subject under test (SUT) from a dependency injection
/// container. Derived classes can use this functionality to access the SUT.</remarks>
/// <typeparam name="TTestType">The type of the subject under test (SUT). Must be a reference type.</typeparam>
public abstract class BaseTestByType<TTestType> : BaseTest where TTestType : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestByType"/> class and registers a logger for the specified
    /// test type.
    /// </summary>
    /// <remarks>This constructor ensures that a logger of type <see cref="ILogger{TTestType}"/> is registered
    /// in the container for the test type. Derived classes can rely on this registration for logging
    /// purposes.</remarks>
    protected BaseTestByType()
    {
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
    /// Gets the logger instance used for logging operations.
    /// </summary>
    public ListLogger<TTestType> Logger => new();
}