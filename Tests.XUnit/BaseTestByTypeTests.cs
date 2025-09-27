using DepenMock.XUnit;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Tests.XUnit;

public class BaseTestByTypeTests
{
    [Fact]
    public void Constructor_ShouldInitializeContainer()
    {
        // Act
        var baseTest = new TestableBaseTestByType();

        // Assert
        Assert.NotNull(baseTest.Container);
    }

    [Fact]
    public void Constructor_ShouldRegisterLogger()
    {
        // Act
        var baseTest = new TestableBaseTestByType();

        // Assert
        Assert.NotNull(baseTest.Logger);
    }

    [Fact]
    public void ResolveSut_ShouldReturnInstanceOfTestType()
    {
        // Arrange
        var baseTest = new TestableBaseTestByType();

        // Act
        var sut = baseTest.ResolveSut();

        // Assert
        Assert.NotNull(sut);
        Assert.IsType<TestClass>(sut);
    }

    [Fact]
    public void ResolveSut_CalledMultipleTimes_ShouldReturnSameInstance()
    {
        // Arrange
        var baseTest = new TestableBaseTestByType();

        // Act
        var first = baseTest.ResolveSut();
        var second = baseTest.ResolveSut();

        // Assert
        Assert.Same(first, second);
    }

    [Fact]
    public void Logger_ShouldBeRegisteredInContainer()
    {
        // Arrange
        var baseTest = new TestableBaseTestByType();

        // Act
        var loggerFromContainer = baseTest.Container.Resolve<ILogger<TestClass>>();

        // Assert
        Assert.Same(baseTest.Logger, loggerFromContainer);
    }

    [Fact]
    public void AddContainerCustomizations_ShouldBeCalled()
    {
        // Act
        var baseTest = new TestableBaseTestByTypeWithCustomizations();

        // Assert
        Assert.True(baseTest.CustomizationsAdded);
    }

    private class TestableBaseTestByType : BaseTestByType<TestClass>
    {
        public new TestClass ResolveSut() => base.ResolveSut();
    }

    private class TestableBaseTestByTypeWithCustomizations : BaseTestByType<TestClass>
    {
        public bool CustomizationsAdded { get; private set; }

        protected override void AddContainerCustomizations(DepenMock.Container container)
        {
            CustomizationsAdded = true;
        }
    }

    public class TestClass
    {
        public string Name { get; set; }
    }
}