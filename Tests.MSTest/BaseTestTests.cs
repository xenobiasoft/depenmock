using DepenMock.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DepenMock.Moq;

namespace Tests.MSTest;

[TestClass]
public class BaseTestTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeContainer()
    {
        // Act
        var baseTest = new TestableBaseTest();

        // Assert
        Assert.IsNotNull(baseTest.Container);
    }

    [TestMethod]
    public void Container_ShouldBeUsableForObjectCreation()
    {
        // Arrange
        var baseTest = new TestableBaseTest();

        // Act
        var result = baseTest.Container.Create<string>();

        // Assert
        Assert.IsNotNull(result);
    }

    private class TestableBaseTest : BaseTest
    {
        public TestableBaseTest() : base(new MoqMockFactory()) { }
    }
}