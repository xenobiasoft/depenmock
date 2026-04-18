using AutoFixture;
using AutoFixture.Kernel;

namespace DepenMock.Mocks;

/// <summary>
/// Defines a factory that integrates a mocking framework with AutoFixture and produces
/// <see cref="IMock{T}"/> wrappers. Implement this interface to swap in a different mocking
/// framework (e.g. NSubstitute, FakeItEasy) without changing any other DepenMock code.
/// </summary>
public interface IMockFactory
{
    /// <summary>
    /// Returns the AutoFixture <see cref="ICustomization"/> that wires the mocking framework into
    /// the fixture so that unregistered dependencies are automatically created as mocks.
    /// </summary>
    ICustomization GetCustomization();

    /// <summary>
    /// Retrieves or creates the mock for <typeparamref name="T"/> from <paramref name="fixture"/>
    /// and returns it wrapped in an <see cref="IMock{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to mock. Must be a reference type.</typeparam>
    /// <param name="fixture">The AutoFixture fixture that owns the mock instance.</param>
    IMock<T> GetMock<T>(IFixture fixture) where T : class;
}
