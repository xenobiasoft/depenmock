using DepenMock.Mocks;
using Moq;

namespace DepenMock.Moq;

/// <summary>
/// Extension methods for working with Moq-backed <see cref="IMock{T}"/> instances.
/// </summary>
public static class MoqExtensions
{
    /// <summary>
    /// Unwraps the underlying Moq <see cref="Mock{T}"/> from an <see cref="IMock{T}"/>, giving
    /// access to Moq's <c>Setup</c> and <c>Verify</c> APIs.
    /// </summary>
    /// <typeparam name="T">The mocked type.</typeparam>
    /// <param name="mock">The <see cref="IMock{T}"/> returned by <c>Container.ResolveMock&lt;T&gt;()</c>.</param>
    /// <returns>The underlying <see cref="Mock{T}"/>.</returns>
    /// <example>
    /// <code>
    /// // Stub
    /// Container.ResolveMock&lt;IDeskRepository&gt;().AsMoq()
    ///     .Setup(x => x.GetAvailableDesks(It.IsAny&lt;DateTime&gt;()))
    ///     .Returns(Container.CreateMany&lt;Desk&gt;());
    ///
    /// // Spy
    /// var mock = Container.ResolveMock&lt;IDeskBookingRepository&gt;().AsMoq();
    /// mock.Verify(x => x.Save(It.IsAny&lt;DeskBooking&gt;()), Times.Once);
    /// </code>
    /// </example>
    public static Mock<T> AsMoq<T>(this Mocks.IMock<T> mock) where T : class =>
        ((MoqMock<T>)mock).Mock;
}
