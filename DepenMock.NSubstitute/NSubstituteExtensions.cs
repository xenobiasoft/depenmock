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
}