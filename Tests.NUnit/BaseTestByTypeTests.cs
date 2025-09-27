using DepenMock.NUnit;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests.NUnit;

[TestFixture]
public class BaseTestByTypeTests
{
    [Test]
    public void Constructor_ShouldInitializeContainer()
    {
        // Act
        var baseTest = new TestableBaseTestByType();

        // Assert
        Assert.That(baseTest.Container, Is.Not.Null);
    }

    [Test]
    public void Constructor_ShouldRegisterLogger()
    {
        // Act
        var baseTest = new TestableBaseTestByType();

        // Assert
        Assert.That(baseTest.Logger, Is.Not.Null);
    }

    [Test]
    public void ResolveSut_ShouldReturnInstanceOfTestType()
    {
        // Arrange
        var baseTest = new TestableBaseTestByType();

        // Act
        var sut = baseTest.ResolveSut();

        // Assert
        Assert.That(sut, Is.Not.Null);
        Assert.That(sut, Is.InstanceOf<TestClass>());
    }

    [Test]
    public void ResolveSut_CalledMultipleTimes_ShouldReturnSameInstance()
    {
        // Arrange
        var baseTest = new TestableBaseTestByType();

        // Act
        var first = baseTest.ResolveSut();
        var second = baseTest.ResolveSut();

        // Assert
        Assert.That(first, Is.SameAs(second));
    }

    [Test]
    public void Logger_ShouldBeRegisteredInContainer()
    {
        // Arrange
        var baseTest = new TestableBaseTestByType();

        // Act
        var loggerFromContainer = baseTest.Container.Resolve<ILogger<TestClass>>();

        // Assert
        Assert.That(loggerFromContainer, Is.SameAs(baseTest.Logger));
    }

    [Test]
    public void AddContainerCustomizations_ShouldBeCalled()
    {
        // Act
        var baseTest = new TestableBaseTestByTypeWithCustomizations();

        // Assert
        Assert.That(baseTest.CustomizationsAdded, Is.True);
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