using DepenMock.NUnit;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests.NUnit;

[TestFixture]
public class BaseTestTests
{
    [Test]
    public void Constructor_ShouldInitializeContainer()
    {
        // Act
        var baseTest = new TestableBaseTest();

        // Assert
        Assert.That(baseTest.Container, Is.Not.Null);
    }

    [Test]
    public void Container_ShouldBeUsableForObjectCreation()
    {
        // Arrange
        var baseTest = new TestableBaseTest();

        // Act
        var result = baseTest.Container.Create<string>();

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    private class TestableBaseTest : BaseTest
    {
        // This class allows us to test the abstract BaseTest class
    }
}