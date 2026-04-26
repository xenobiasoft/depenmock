using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using DepenMock.Mocks;

namespace DepenMock.FakeItEasy;

/// <summary>
/// An <see cref="IMockFactory"/> implementation that uses FakeItEasy and AutoFixture.AutoFakeItEasy to
/// automatically create and register fakes for unregistered dependencies.
/// </summary>
public class FakeItEasyMockFactory : IMockFactory
{
    /// <inheritdoc />
    /// <remarks>
    /// Returns an <see cref="AutoFakeItEasyCustomization"/> with <c>ConfigureMembers = true</c>, which
    /// instructs AutoFixture to automatically configure all members of auto-created fakes.
    /// </remarks>
    public ICustomization GetCustomization() => new AutoFakeItEasyCustomization { ConfigureMembers = true };

    /// <inheritdoc />
    /// <remarks>
    /// Freezes <typeparamref name="T"/> in the fixture so the same fake instance is reused whenever
    /// <typeparamref name="T"/> is resolved, then wraps it in a <see cref="FakeItEasyMock{T}"/>.
    /// </remarks>
    public IMock<T> GetMock<T>(IFixture fixture) where T : class => new FakeItEasyMock<T>(fixture.Freeze<T>());
}
