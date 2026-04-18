using AutoFixture;
using AutoFixture.AutoMoq;
using DepenMock.Mocks;
using Moq;

namespace DepenMock.Moq;

/// <summary>
/// An <see cref="IMockFactory"/> implementation that uses Moq and AutoFixture.AutoMoq to
/// automatically create and register mocks for unregistered dependencies.
/// </summary>
public class MoqMockFactory : IMockFactory
{
    /// <inheritdoc />
    /// <remarks>
    /// Returns an <see cref="AutoMoqCustomization"/> with <c>ConfigureMembers = true</c>, which
    /// instructs AutoFixture to automatically stub all members of auto-created mocks.
    /// </remarks>
    public ICustomization GetCustomization() => new AutoMoqCustomization { ConfigureMembers = true };

    /// <inheritdoc />
    /// <remarks>
    /// Freezes a <see cref="Mock{T}"/> in the fixture so the same instance is reused whenever
    /// <typeparamref name="T"/> is resolved, then wraps it in a <see cref="MoqMock{T}"/>.
    /// </remarks>
    public Mocks.IMock<T> GetMock<T>(IFixture fixture) where T : class => new MoqMock<T>(fixture.Freeze<Mock<T>>());
}
