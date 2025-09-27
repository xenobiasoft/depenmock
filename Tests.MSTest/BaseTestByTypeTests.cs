using DepenMock.MSTest;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MSTest;

[TestClass]
public class BaseTestByTypeTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeContainer()
    {
        // Act
        var baseTest = new TestableBaseTestByType();

        // Assert
        Assert.IsNotNull(baseTest.Container);
    }

    [TestMethod]
    public void Constructor_ShouldRegisterLogger()
    {
        // Act
        var baseTest = new TestableBaseTestByType();

        // Assert
        Assert.IsNotNull(baseTest.Logger);
    }

    [TestMethod]
    public void ResolveSut_ShouldReturnInstanceOfTestType()
    {
        // Arrange
        var baseTest = new TestableBaseTestByType();

        // Act
        var sut = baseTest.ResolveSut();

        // Assert
        Assert.IsNotNull(sut);
        Assert.IsInstanceOfType(sut, typeof(TestClass));
    }

    [TestMethod]
    public void ResolveSut_CalledMultipleTimes_ShouldReturnSameInstance()
    {
        // Arrange
        var baseTest = new TestableBaseTestByType();

        // Act
        var first = baseTest.ResolveSut();
        var second = baseTest.ResolveSut();

        // Assert
        Assert.AreSame(first, second);
    }

    [TestMethod]
    public void Logger_ShouldBeRegisteredInContainer()
    {
        // Arrange
        var baseTest = new TestableBaseTestByType();

        // Act
        var loggerFromContainer = baseTest.Container.Resolve<ILogger<TestClass>>();

        // Assert
        Assert.AreSame(baseTest.Logger, loggerFromContainer);
    }

    [TestMethod]
    public void AddContainerCustomizations_ShouldBeCalled()
    {
        // Act
        var baseTest = new TestableBaseTestByTypeWithCustomizations();

        // Assert
        Assert.IsTrue(baseTest.CustomizationsAdded);
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