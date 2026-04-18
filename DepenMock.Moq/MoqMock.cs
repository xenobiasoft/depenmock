using DepenMock.Mocks;
using Moq;

namespace DepenMock.Moq;

/// <summary>
/// An <see cref="IMock{T}"/> implementation backed by a Moq <see cref="Mock{T}"/>. Access the
/// underlying <see cref="Mock"/> property (or call <see cref="MoqExtensions.AsMoq{T}"/>) to use
/// Moq's <c>Setup</c> and <c>Verify</c> APIs.
/// </summary>
/// <typeparam name="T">The type being mocked. Must be a reference type.</typeparam>
public class MoqMock<T> : Mocks.IMock<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of <see cref="MoqMock{T}"/> wrapping the given Moq mock.
    /// </summary>
    /// <param name="mock">The underlying Moq <see cref="Mock{T}"/> instance.</param>
    public MoqMock(Mock<T> mock)
    {
        Mock = mock;
    }

    /// <inheritdoc />
    public T Object => Mock.Object;

    /// <summary>
    /// Gets the underlying Moq <see cref="Mock{T}"/>, providing access to <c>Setup</c>,
    /// <c>Verify</c>, and all other Moq-specific APIs.
    /// </summary>
    public Mock<T> Mock { get; }
}
