using DepenMock.Mocks;

namespace DepenMock.FakeItEasy;

/// <summary>
/// An <see cref="IMock{T}"/> implementation backed by a FakeItEasy fake. Access the
/// underlying <see cref="Fake"/> property (or call <see cref="FakeItEasyExtensions.AsFake{T}"/>)
/// to use FakeItEasy's <c>A.CallTo()</c> API.
/// </summary>
/// <typeparam name="T">The type being faked. Must be a reference type.</typeparam>
public class FakeItEasyMock<T> : IMock<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of <see cref="FakeItEasyMock{T}"/> wrapping the given fake.
    /// </summary>
    /// <param name="fake">The underlying FakeItEasy fake instance.</param>
    public FakeItEasyMock(T fake)
    {
        Fake = fake;
    }

    /// <inheritdoc />
    public T Object => Fake;

    /// <summary>
    /// Gets the underlying FakeItEasy fake instance, providing access to
    /// <c>A.CallTo()</c> for setup and assertion.
    /// </summary>
    public T Fake { get; }
}
