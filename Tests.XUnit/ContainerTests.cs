using AutoFixture;
using AutoFixture.Kernel;
using Moq;
using Xunit;

namespace Tests.XUnit;

public class ContainerTests
{
    private readonly DepenMock.Container _container;

    public ContainerTests()
    {
        _container = new DepenMock.Container();
    }

    [Fact]
    public void Create_WithPrimitiveType_ShouldReturnValue()
    {
        // Act
        var result = _container.Create<int>();

        // Assert
        Assert.NotEqual(0, result);
    }

    [Fact]
    public void Create_WithString_ShouldReturnNonNullString()
    {
        // Act
        var result = _container.Create<string>();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    [Fact]
    public void Create_WithComplexType_ShouldReturnInstanceWithPopulatedProperties()
    {
        // Act
        var result = _container.Create<TestModel>();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Name);
        Assert.NotEqual(0, result.Id);
    }

    [Fact]
    public void CreateMany_WithDefaultCount_ShouldReturnThreeItems()
    {
        // Act
        var results = _container.CreateMany<int>();

        // Assert
        Assert.Equal(3, results.Count());
    }

    [Fact]
    public void CreateMany_WithSpecificCount_ShouldReturnSpecifiedNumberOfItems()
    {
        // Arrange
        var expectedCount = 5;

        // Act
        var results = _container.CreateMany<int>(expectedCount);

        // Assert
        Assert.Equal(expectedCount, results.Count());
    }

    [Fact]
    public void Build_ShouldReturnCustomizationComposer()
    {
        // Act
        var composer = _container.Build<TestModel>();

        // Assert
        Assert.NotNull(composer);
    }

    [Fact]
    public void Build_WithCustomization_ShouldRespectCustomization()
    {
        // Arrange
        var customName = "CustomName";

        // Act
        var result = _container.Build<TestModel>()
            .With(x => x.Name, customName)
            .Create();

        // Assert
        Assert.Equal(customName, result.Name);
    }

    [Fact]
    public void Resolve_ShouldReturnSameInstanceOnMultipleCalls()
    {
        // Act
        var first = _container.Resolve<TestModel>();
        var second = _container.Resolve<TestModel>();

        // Assert
        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.Same(first, second);
    }

    [Fact]
    public void ResolveMock_ShouldReturnMockInstance()
    {
        // Act
        var mock = _container.ResolveMock<ITestService>();

        // Assert
        Assert.NotNull(mock);
        Assert.NotNull(mock.Object);
    }

    [Fact]
    public void ResolveMock_ShouldReturnSameMockOnMultipleCalls()
    {
        // Act
        var first = _container.ResolveMock<ITestService>();
        var second = _container.ResolveMock<ITestService>();

        // Assert
        Assert.Same(first, second);
    }

    [Fact]
    public void Register_WithInstance_ShouldReturnRegisteredInstance()
    {
        // Arrange
        var instance = new TestModel { Name = "RegisteredInstance", Id = 123 };

        // Act
        _container.Register(instance);
        var result = _container.Resolve<TestModel>();

        // Assert
        Assert.Same(instance, result);
        Assert.Equal("RegisteredInstance", result.Name);
        Assert.Equal(123, result.Id);
    }

    [Fact]
    public void Register_WithInterfaceAndInstance_ShouldReturnRegisteredInstance()
    {
        // Arrange
        var instance = new TestService();

        // Act
        _container.Register<ITestService, TestService>(instance);
        var result = _container.Resolve<ITestService>();

        // Assert
        Assert.Same(instance, result);
    }

    [Fact]
    public void AddCustomizations_WithValidBuilders_ShouldAddToCustomizations()
    {
        // Arrange
        var builder = new Mock<ISpecimenBuilder>();

        // Act
        var exception = Record.Exception(() => _container.AddCustomizations(builder.Object));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void AddCustomizations_WithNullBuilders_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _container.AddCustomizations(null));
    }

    [Fact]
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