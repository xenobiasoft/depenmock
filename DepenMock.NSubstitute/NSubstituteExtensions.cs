using DepenMock.Mocks;

namespace DepenMock.NSubstitute;

/// <summary>
/// Extension methods for working with NSubstitute-backed <see cref="IMock{T}"/> instances.
/// </summary>
public static class NSubstituteExtensions
{
    /// <summary>
    /// Unwraps the underlying NSubstitute substitute from an <see cref="IMock{T}"/>, giving
    /// access to NSubstitute's <c>Returns</c> and <c>Received</c> APIs.
    /// </summary>
    /// <typeparam name="T">The mocked type.</typeparam>
    /// <param name="mock">The <see cref="IMock{T}"/> returned by <c>Container.ResolveMock&lt;T&gt;()</c>.</param>
    /// <returns>The underlying NSubstitute substitute instance.</returns>
    /// <example>
    /// <code>
    /// // Stub
    /// Container.ResolveMock&lt;IDeskRepository&gt;().AsNSubstitute()
    ///     .GetAvailableDesks(Arg.Any&lt;DateTime&gt;())
    ///     .Returns(Container.CreateMany&lt;Desk&gt;());
    ///
    /// // Spy
    /// Container.ResolveMock&lt;IDeskBookingRepository&gt;().AsNSubstitute()
    ///     .Received(1).Save(Arg.Any&lt;DeskBooking&gt;());
    /// </code>
    /// </example>
    public static T AsNSubstitute<T>(this IMock<T> mock) where T : class =>
        ((NSubstituteMock<T>)mock).Substitute;

    /// <summary>
    /// Resolves the mock for <typeparamref name="T"/> and applies <paramref name="setup"/> to the
    /// underlying NSubstitute substitute inline, without needing an intermediate variable. The
    /// substitute is the same cached instance returned by <c>Container.ResolveMock&lt;T&gt;()</c>,
    /// so any configuration applied here is visible to the system under test.
    /// </summary>
    /// <typeparam name="T">The mocked type.</typeparam>
    /// <param name="container">The container that owns the mock.</param>
    /// <param name="setup">The configuration to apply to the underlying substitute.</param>
    /// <returns>The same <see cref="Container"/>, so that calls can be chained.</returns>
    /// <example>
    /// <code>
    /// Container
    ///     .SetupMock&lt;IDeskRepository&gt;(s => s
    ///         .GetAvailableDesks(Arg.Any&lt;DateTime&gt;())
    ///         .Returns(Container.CreateMany&lt;Desk&gt;()))
    ///     .SetupMock&lt;IDeskBookingRepository&gt;(s => s
    ///         .When(x => x.Save(Arg.Any&lt;DeskBooking&gt;()))
    ///         .Do(_ => throw new InvalidOperationException()));
    /// </code>
    /// </example>
    public static Container SetupMock<T>(this Container container, Action<T> setup) where T : class
    {
        ArgumentNullException.ThrowIfNull(setup);

        setup(container.ResolveMock<T>().AsNSubstitute());

        return container;
    }
}