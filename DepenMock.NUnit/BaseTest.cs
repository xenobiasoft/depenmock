using NUnit.Framework;

namespace DepenMock.NUnit;

/// <summary>
/// Represents the base class for test fixtures that require a dependency injection container.
/// </summary>
/// <remarks>This class provides a common setup mechanism for initializing a <see cref="Container"/> instance
/// for each test fixture. Derived classes can use the <see cref="Container"/> property to register and resolve
/// dependencies.</remarks>
public abstract class BaseTest
{
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTest"/> class.
    /// </summary>
    /// <remarks>This constructor initializes the <see cref="Container"/> property and performs initial setup.
    /// The setup is also performed in the SetUp method to maintain NUnit lifecycle compatibility.</remarks>
    protected BaseTest()
    {
        InitializeContainer();
    }

    /// <summary>
    /// Sets up the test environment by initializing the dependency injection container.
    /// </summary>
    /// <remarks>This method is executed before each test to ensure a fresh instance of the container. It is
    /// marked with the <see cref="SetUpAttribute"/> to indicate that it should run prior to each test in
    /// NUnit.</remarks>
    [SetUp]
    public void BaseSetup()
    {
        InitializeContainer();
    }

    /// <summary>
    /// Performs cleanup operations after each test execution.
    /// </summary>
    /// <remarks>This method resets the test container and marks the test as uninitialized.  It is executed
    /// automatically after each test due to the <see cref="TearDownAttribute"/>.</remarks>
    [TearDown]
    public void BaseTearDown()
    {
        Container = null;
        _isInitialized = false;
    }

    /// <summary>
    /// Initializes the container
    /// </summary>
    private void InitializeContainer()
    {
        if (!_isInitialized)
        {
            Container = new Container();
            _isInitialized = true;
        }
    }

    /// <summary>
    /// Gets the container instance associated with the current object.
    /// </summary>
    public Container Container { get; private set; }
}