using System;
using System.Linq;
using AutoFixture.Kernel;
using Moq;
using NUnit.Framework;
using AutoFixture;

namespace Tests.NUnit;

[TestFixture]
public class ContainerTests
{
    private DepenMock.Container _container;

    [SetUp]
    public void Setup()
    {
        _container = new DepenMock.Container();
    }

    [Test]
    public void Create_WithPrimitiveType_ShouldReturnValue()
    {
        // Act
        var result = _container.Create<int>();

        // Assert
        Assert.That(result, Is.Not.EqualTo(0));
    }

    [Test]
    public void Create_WithString_ShouldReturnNonNullString()
    {
        // Act
        var result = _container.Create<string>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Length, Is.GreaterThan(0));
    }

    [Test]
    public void Create_WithComplexType_ShouldReturnInstanceWithPopulatedProperties()
    {
        // Act
        var result = _container.Create<TestModel>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.Not.Null);
        Assert.That(result.Id, Is.Not.EqualTo(0));
    }

    [Test]
    public void CreateMany_WithDefaultCount_ShouldReturnThreeItems()
    {
        // Act
        var results = _container.CreateMany<int>();

        // Assert
        Assert.That(results.Count(), Is.EqualTo(3));
    }

    [Test]
    public void CreateMany_WithSpecificCount_ShouldReturnSpecifiedNumberOfItems()
    {
        // Arrange
        var expectedCount = 5;

        // Act
        var results = _container.CreateMany<int>(expectedCount);

        // Assert
        Assert.That(results.Count(), Is.EqualTo(expectedCount));
    }

    [Test]
    public void Build_ShouldReturnCustomizationComposer()
    {
        // Act
        var composer = _container.Build<TestModel>();

        // Assert
        Assert.That(composer, Is.Not.Null);
    }

    [Test]
    public void Build_WithCustomization_ShouldRespectCustomization()
    {
        // Arrange
        var customName = "CustomName";

        // Act
        var result = _container.Build<TestModel>()
            .With(x => x.Name, customName)
            .Create();

        // Assert
        Assert.That(result.Name, Is.EqualTo(customName));
    }

    [Test]
    public void Resolve_ShouldReturnSameInstanceOnMultipleCalls()
    {
        // Act
        var first = _container.Resolve<TestModel>();
        var second = _container.Resolve<TestModel>();

        // Assert
        Assert.That(first, Is.Not.Null);
        Assert.That(second, Is.Not.Null);
        Assert.That(first, Is.SameAs(second));
    }

    [Test]
    public void ResolveMock_ShouldReturnMockInstance()
    {
        // Act
        var mock = _container.ResolveMock<ITestService>();

        // Assert
        Assert.That(mock, Is.Not.Null);
        Assert.That(mock.Object, Is.Not.Null);
    }

    [Test]
    public void ResolveMock_ShouldReturnSameMockOnMultipleCalls()
    {
        // Act
        var first = _container.ResolveMock<ITestService>();
        var second = _container.ResolveMock<ITestService>();

        // Assert
        Assert.That(first, Is.SameAs(second));
    }

    [Test]
    public void Register_WithInstance_ShouldReturnRegisteredInstance()
    {
        // Arrange
        var instance = new TestModel { Name = "RegisteredInstance", Id = 123 };

        // Act
        _container.Register(instance);
        var result = _container.Resolve<TestModel>();

        // Assert
        Assert.That(result, Is.SameAs(instance));
        Assert.That(result.Name, Is.EqualTo("RegisteredInstance"));
        Assert.That(result.Id, Is.EqualTo(123));
    }

    [Test]
    public void Register_WithInterfaceAndInstance_ShouldReturnRegisteredInstance()
    {
        // Arrange
        var instance = new TestService();

        // Act
        _container.Register<ITestService, TestService>(instance);
        var result = _container.Resolve<ITestService>();

        // Assert
        Assert.That(result, Is.SameAs(instance));
    }

    [Test]
    public void AddCustomizations_WithValidBuilders_ShouldAddToCustomizations()
    {
        // Arrange
        var builder = new Mock<ISpecimenBuilder>();

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => _container.AddCustomizations(builder.Object));
    }

    [Test]
    public void AddCustomizations_WithNullBuilders_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _container.AddCustomizations(null));
    }

    [Test]
    public void AddCustomizations_WithNullBuilderInArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        var validBuilder = new Mock<ISpecimenBuilder>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            _container.AddCustomizations(validBuilder.Object, null));
    }

    public class TestModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public interface ITestService
    {
        void DoSomething();
    }

    public class TestService : ITestService
    {
        public void DoSomething()
        {
            // Implementation
        }
    }
}