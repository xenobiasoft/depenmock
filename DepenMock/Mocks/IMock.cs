namespace DepenMock.Mocks;

/// <summary>
/// Represents a mock object wrapping an instance of <typeparamref name="T"/>, decoupled from any
/// specific mocking framework. Use <c>AsMoq()</c> (from the <c>DepenMock.Moq</c> package) to access
/// Moq-specific setup and verification APIs.
/// </summary>
/// <typeparam name="T">The type being mocked. Must be a reference type.</typeparam>
public interface IMock<out T> where T : class
{
    /// <summary>
    /// Gets the mocked instance of <typeparamref name="T"/> that can be passed to the system under test.
    /// </summary>
    T Object { get; }
}
