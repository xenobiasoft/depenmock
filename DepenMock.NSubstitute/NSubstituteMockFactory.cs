using AutoFixture;
using AutoFixture.AutoNSubstitute;
using DepenMock.Mocks;

namespace DepenMock.NSubstitute;

/// <summary>
/// An <see cref="IMockFactory"/> implementation that uses NSubstitute and AutoFixture.AutoNSubstitute to
/// automatically create and register substitutes for unregistered dependencies.
/// </summary>
public class NSubstituteMockFactory : IMockFactory
{
    /// <inheritdoc />
    /// <remarks>
    /// Returns an <see cref="AutoNSubstituteCustomization"/> with <c>ConfigureMembers = true</c>, which
    /// instructs AutoFixture to automatically stub all members of auto-created substitutes.
    /// </remarks>
    public ICustomization GetCustomization() => new AutoNSubstituteCustomization { ConfigureMembers = true };

    /// <inheritdoc />
    /// <remarks>
    /// Freezes <typeparamref name="T"/> in the fixture so the same substitute instance is reused whenever
    /// <typeparamref name="T"/> is resolved, then wraps it in a <see cref="NSubstituteMock{T}"/>.
    /// </remarks>
    public IMock<T> GetMock<T>(IFixture fixture) where T : class => new NSubstituteMock<T>(fixture.Freeze<T>());
}