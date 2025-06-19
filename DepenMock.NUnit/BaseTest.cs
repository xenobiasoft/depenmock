using NUnit.Framework;

namespace DepenMock.NUnit;

/// <summary>
/// Represents the base class for test fixtures that require a dependency injection container.
/// </summary>
/// <remarks>This class provides a common setup mechanism for initializing a <see cref="Container"/> instance
/// before each test execution. Derived classes can use the <see cref="Container"/> property to register and resolve
/// dependencies.</remarks>
public abstract class BaseTest
{
    /// <summary>
    /// Sets up the test environment by initializing the dependency injection container.
    /// </summary>
    /// <remarks>This method is executed before each test to ensure a fresh instance of the container. It is
    /// marked with the <see cref="SetUpAttribute"/> to indicate that it should run prior to each test in
    /// NUnit.</remarks>
    [SetUp]
    public void BaseSetup()
    {
        Container = new Container();
    }

    /// <summary>
    /// Gets the container instance associated with the current object.
    /// </summary>
    public Container Container { get; private set; }
}