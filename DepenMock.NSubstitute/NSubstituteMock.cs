using DepenMock.Mocks;

namespace DepenMock.NSubstitute;

/// <summary>
/// An <see cref="IMock{T}"/> implementation backed by an NSubstitute substitute. Access the
/// underlying <see cref="Substitute"/> property (or call <see cref="NSubstituteExtensions.AsNSubstitute{T}"/>)
/// to use NSubstitute's <c>Returns</c> and <c>Received</c> APIs.
/// </summary>
/// <typeparam name="T">The type being mocked. Must be a reference type.</typeparam>
public class NSubstituteMock<T> : IMock<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of <see cref="NSubstituteMock{T}"/> wrapping the given substitute.
    /// </summary>
    /// <param name="substitute">The underlying NSubstitute instance.</param>
    public NSubstituteMock(T substitute)
    {
        Substitute = substitute;
    }

    /// <inheritdoc />
    public T Object => Substitute;

    /// <summary>
    /// Gets the underlying NSubstitute instance, providing access to <c>Returns</c>,
    /// <c>Received</c>, and all other NSubstitute-specific APIs.
    /// </summary>
    public T Substitute { get; }
}