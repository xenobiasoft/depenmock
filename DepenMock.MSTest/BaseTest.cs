namespace DepenMock.MSTest;

/// <summary>
/// Represents the base class for test scenarios, providing common functionality and shared resources.
/// </summary>
/// <remarks>The <see cref="BaseTest"/> class serves as a foundation for creating test cases. It
/// initializes  a <see cref="Container"/> instance to manage core services and dependencies, which can be used  by
/// derived classes to set up and execute testing scenarios.</remarks>
public abstract class BaseTest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTest"/> class.
    /// </summary>
    /// <remarks>This constructor initializes the <see cref="Container"/> property with a new instance
    /// of the <see cref="Container"/> class. Derived classes can use this constructor to set up the base state for
    /// testing scenarios.</remarks>
    protected BaseTest()
    {
        Container = new Container();
    }

    /// <summary>
    /// Gets the container that holds the application's core services and dependencies.
    /// </summary>
    public Container Container { get; }
}