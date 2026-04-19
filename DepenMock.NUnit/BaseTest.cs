using DepenMock.Mocks;
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
    private readonly IMockFactory _mockFactory;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTest"/> class using the specified mock factory.
    /// </summary>
    /// <param name="mockFactory">
    /// The mock factory that integrates a mocking framework with AutoFixture. Pass <c>new MoqMockFactory()</c>
    /// from <c>DepenMock.Moq</c> or <c>new NSubstituteMockFactory()</c> from <c>DepenMock.NSubstitute</c>.
    /// </param>
    protected BaseTest(IMockFactory mockFactory)
    {
        _mockFactory = mockFactory;
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
            Container = new Container(_mockFactory);
            _isInitialized = true;
        }
    }

    /// <summary>
    /// Gets the container instance associated with the current object.
    /// </summary>
    public Container Container { get; private set; }
}