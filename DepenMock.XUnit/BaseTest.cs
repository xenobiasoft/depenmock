using DepenMock.Mocks;

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
    /// Initializes a new instance of the <see cref="BaseTest"/> class using the specified mock factory.
    /// </summary>
    /// <param name="mockFactory">
    /// The mock factory that integrates a mocking framework with AutoFixture. Pass <c>new MoqMockFactory()</c>
    /// from <c>DepenMock.Moq</c> or <c>new NSubstituteMockFactory()</c> from <c>DepenMock.NSubstitute</c>.
    /// </param>
    protected BaseTest(IMockFactory mockFactory)
    {
        Container = new Container(mockFactory);
    }

    /// <summary>
    /// Gets the container instance associated with the current context.
    /// </summary>
    public Container Container { get; }
}