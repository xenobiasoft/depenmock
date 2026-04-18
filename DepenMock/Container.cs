using System;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using System.Collections.Generic;
using System.Linq;
using DepenMock.Customizations;
using DepenMock.Mocks;

namespace DepenMock;

/// <summary>
/// Provides object creation functionality, as well as acting as a pseudo dependency injection container
/// </summary>
public class Container
{
    private readonly IFixture _fixture;
    private readonly IMockFactory _mockFactory;
    private readonly Dictionary<Type, object> _mockCache = new();

    /// <summary>
    /// Initializes a new instance of <see cref="Container"/> using the provided mock factory.
    /// </summary>
    /// <param name="mockFactory">
    /// The factory that integrates a mocking framework with AutoFixture and produces
    /// <see cref="IMock{T}"/> wrappers. Use <c>new MoqMockFactory()</c> from the
    /// <c>DepenMock.Moq</c> package for the default Moq-backed implementation.
    /// </param>
    public Container(IMockFactory mockFactory)
    {
        _mockFactory = mockFactory;
        _fixture = new Fixture().Customize(mockFactory.GetCustomization());

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(x => _fixture.Behaviors.Remove(x));
        _fixture
            .Customize(new ControllerCustomization())
            .Behaviors.Add(new OmitOnRecursionBehavior());
    }

    /// <summary>
    /// Creates a new instance of the specified type. This acts as a data generator for
    /// primitive types and strings. For object types, it will generate a new instance
    /// of the class with all properties set with generated data.
    /// </summary>
    /// <typeparam name="TType">Any primitive type, string, or object</typeparam>
    /// <returns>A new instance of the requested type.</returns>
    public TType Create<TType>()
    {
        return _fixture.Create<TType>();
    }

    /// <summary>
    /// Creates a list of specified type. This acts as a data generator for
    /// primitive types and strings. For object types, it will generate a list of instances
    /// of the specified type with all properties set with generated data.
    /// </summary>
    /// <typeparam name="TType">Any primitive type, string, or object</typeparam>
    /// <param name="count">The number of instances to create</param>
    /// <returns>A list of instances of the requested type.</returns>
    public IList<TType> CreateMany<TType>(int? count = 3)
    {
        return _fixture.CreateMany<TType>(count.GetValueOrDefault()).ToList();
    }

    /// <summary>
    /// Customizes the creation process for a single object.
    /// </summary>
    /// <typeparam name="TType">Any object type</typeparam>
    /// <returns>
    /// A <see cref="T:AutoFixture.Dsl.ICustomizationComposer`1" /> that can be used to customize the creation
    /// algorithm before creating the object.
    /// </returns>
    public ICustomizationComposer<TType> Build<TType>()
    {
        return _fixture.Build<TType>();
    }

    /// <summary>
    /// Creates a new instance of the requested type and registers it in the Fixture.
    /// The fixture always returns the same instance whenever the instance of the type is requested
    /// either directly, or indirectly as a nested value of other types.
    /// </summary>
    /// <typeparam name="TType">Any object type</typeparam>
    /// <returns>A new instance of the requested type.</returns>
    public TType Resolve<TType>() where TType : class
    {
        return _fixture.Freeze<TType>();
    }

    /// <summary>
    /// Retrieves or creates the mock for <typeparamref name="TType"/> and registers it in the fixture
    /// so that the same instance is returned whenever the type is requested, either directly or as a
    /// nested dependency. Repeated calls for the same type always return the same
    /// <see cref="IMock{TType}"/> wrapper. All properties and methods are automatically configured
    /// by the underlying mocking framework, but can be overridden via the returned wrapper.
    /// Use <c>AsMoq()</c> from the <c>DepenMock.Moq</c> package to access Moq-specific
    /// <c>Setup</c> and <c>Verify</c> APIs.
    /// </summary>
    /// <typeparam name="TType">Any reference type to mock.</typeparam>
    /// <returns>
    /// The same <see cref="IMock{TType}"/> wrapper on every call for a given
    /// <typeparamref name="TType"/> within this container instance.
    /// </returns>
    public IMock<TType> ResolveMock<TType>() where TType : class
    {
        var key = typeof(TType);
        if (!_mockCache.TryGetValue(key, out var cached))
        {
            cached = _mockFactory.GetMock<TType>(_fixture);
            _mockCache[key] = cached;
        }
        return (IMock<TType>)cached;
    }

    /// <summary>
    /// Registers an instance of TInstanceType in the Fixture. The fixture always returns this same instance when the instance
    /// of the type is requested either directly, or indirectly as a nested value of other types.
    /// </summary>
    /// <typeparam name="TInstanceType">The interface type</typeparam>
    /// <param name="instance">The instance of the type that will be registered with the Fixture</param>
    public void Register<TInstanceType>(TInstanceType instance) where TInstanceType : class
    {
        _fixture.Register(() => instance);
    }

    /// <summary>
    /// Registers an instance of a specified type for use in dependency injection or testing scenarios.
    /// </summary>
    /// <remarks>This method associates the provided instance with the specified interface or base type, allowing
    /// it to be resolved in contexts where <typeparamref name="TInterfaceType"/> is required.</remarks>
    /// <typeparam name="TInterfaceType">The interface or base type that the instance implements or inherits.</typeparam>
    /// <typeparam name="TInstanceType">The concrete type of the instance being registered. Must be a class that implements or inherits <typeparamref
    /// name="TInterfaceType"/>.</typeparam>
    /// <param name="instance">The instance to register. Cannot be <see langword="null"/>.</param>
    public void Register<TInterfaceType, TInstanceType>(TInstanceType instance) where TInstanceType : class, TInterfaceType
    {
        _fixture.Register<TInterfaceType>(() => instance);
    }

    /// <summary>
    /// Adds custom ISpecimenBuilder instances to the fixture's Customizations collection.
    /// </summary>
    /// <param name="builders">One or more ISpecimenBuilder instances to add.</param>
    public void AddCustomizations(params ISpecimenBuilder[] builders)
    {
        if (builders == null)
        {
            throw new ArgumentNullException(nameof(builders), "The builders parameter cannot be null.");
        }

        if (builders.Any(builder => builder == null))
        {
            throw new ArgumentNullException(nameof(builders), "None of the builders can be null.");
        }
        foreach (var builder in builders)
        {
            _fixture.Customizations.Add(builder);
        }
    }
}