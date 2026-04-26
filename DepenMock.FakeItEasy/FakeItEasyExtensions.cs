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
}
