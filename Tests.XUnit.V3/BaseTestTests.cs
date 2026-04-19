using DepenMock.XUnit.V3;
using Xunit;
using DepenMock.Moq;

namespace Tests.XUnit.V3;

public class BaseTestTests
{
    [Fact]
    public void Constructor_ShouldInitializeContainer()
    {
        // Act
        var baseTest = new TestableBaseTest();

        // Assert
        Assert.NotNull(baseTest.Container);
    }

    [Fact]
    public void Container_ShouldBeUsableForObjectCreation()
    {
        // Arrange
        var baseTest = new TestableBaseTest();

        // Act
        var result = baseTest.Container.Create<string>();

        // Assert
        Assert.NotNull(result);
    }

    private class TestableBaseTest : BaseTest
    {
        public TestableBaseTest() : base(new MoqMockFactory()) { }
    }
}