using DepenMock.Mocks;

namespace DepenMock.FakeItEasy;

/// <summary>
/// Extension methods for working with FakeItEasy-backed <see cref="IMock{T}"/> instances.
/// </summary>
public static class FakeItEasyExtensions
{
    /// <summary>
    /// Unwraps the underlying FakeItEasy fake from an <see cref="IMock{T}"/>, giving
    /// access to FakeItEasy's <c>A.CallTo()</c> API for setup and assertion.
    /// </summary>
    /// <typeparam name="T">The mocked type.</typeparam>
    /// <param name="mock">The <see cref="IMock{T}"/> returned by <c>Container.ResolveMock&lt;T&gt;()</c>.</param>
    /// <returns>The underlying FakeItEasy fake instance.</returns>
    /// <example>
    /// <code>
    /// // Stub
    /// var fakeRepo = Container.ResolveMock&lt;IDeskRepository&gt;().AsFake();
    /// A.CallTo(() => fakeRepo.GetAvailableDesks(A&lt;DateTime&gt;._))
    ///     .Returns(Container.CreateMany&lt;Desk&gt;());
    ///
    /// // Spy
    /// var fakeBookingRepo = Container.ResolveMock&lt;IDeskBookingRepository&gt;().AsFake();
    /// A.CallTo(() => fakeBookingRepo.Save(A&lt;DeskBooking&gt;._)).MustHaveHappenedOnceExactly();
    /// </code>
    /// </example>
    public static T AsFake<T>(this IMock<T> mock) where T : class =>
        ((FakeItEasyMock<T>)mock).Fake;

    /// <summary>
    /// Resolves the mock for <typeparamref name="T"/> and applies <paramref name="setup"/> to the
    /// underlying FakeItEasy fake inline, without needing an intermediate variable. The fake is the
    /// same cached instance returned by <c>Container.ResolveMock&lt;T&gt;()</c>, so any
    /// configuration applied here is visible to the system under test.
    /// </summary>
    /// <typeparam name="T">The faked type.</typeparam>
    /// <param name="container">The container that owns the fake.</param>
    /// <param name="setup">The configuration to apply to the underlying fake.</param>
    /// <returns>The same <see cref="Container"/>, so that calls can be chained.</returns>
    /// <example>
    /// <code>
    /// Container
    ///     .SetupMock&lt;IDeskRepository&gt;(f => A
    ///         .CallTo(() => f.GetAvailableDesks(A&lt;DateTime&gt;._))
    ///         .Returns(Container.CreateMany&lt;Desk&gt;()))
    ///     .SetupMock&lt;IDeskBookingRepository&gt;(f => A
    ///         .CallTo(() => f.Save(A&lt;DeskBooking&gt;._))
    ///         .DoesNothing());
    /// </code>
    /// </example>
    public static Container SetupMock<T>(this Container container, Action<T> setup) where T : class
    {
        ArgumentNullException.ThrowIfNull(setup);

        setup(container.ResolveMock<T>().AsFake());

        return container;
    }
}
