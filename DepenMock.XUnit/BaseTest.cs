namespace DepenMock.XUnit;

/// <summary>
/// Serves as a base class for test implementations, providing shared functionality and a dependency injection
/// container.
/// </summary>
/// <remarks>The <see cref="BaseTest"/> class is designed to be inherited by test classes that require a common
/// setup or shared resources. It initializes a <see cref="Container"/> instance, which can be used for dependency
/// injection or other purposes. Derived classes can access the <see cref="Container"/> property to manage dependencies
/// within the test context.</remarks>
public abstract class BaseTest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTest"/> class.
    /// </summary>
    /// <remarks>This constructor sets up the <see cref="Container"/> property with a new instance of the <see
    /// cref="Container"/> class. Derived classes can use this container for dependency injection or other
    /// purposes.</remarks>
    protected BaseTest()
    {
        Container = new Container();
    }

    /// <summary>
    /// Gets the container instance associated with the current context.
    /// </summary>
    public Container Container { get; }
}