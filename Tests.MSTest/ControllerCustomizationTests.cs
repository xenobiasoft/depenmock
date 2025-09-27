using AutoFixture;
using AutoFixture.AutoMoq;
using DepenMock.Customizations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MSTest;

[TestClass]
public class ControllerCustomizationTests
{
    [TestMethod]
    public void Customize_ShouldAddControllerSpecification()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var customization = new ControllerCustomization();

        // Act
        customization.Customize(fixture);

        // Assert
        // The test passes if no exception is thrown during customization
        Assert.IsNotNull(customization);
    }

    [TestMethod]
    public void Customize_WithControllerType_ShouldCreateControllerWithHttpContext()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var customization = new ControllerCustomization();
        customization.Customize(fixture);

        // Act
        var controller = fixture.Create<TestController>();

        // Assert
        Assert.IsNotNull(controller);
        Assert.IsNotNull(controller.ControllerContext);
        Assert.IsNotNull(controller.ControllerContext.HttpContext);
    }

    [TestMethod]
    public void Customize_WithNonControllerType_ShouldWorkNormally()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var customization = new ControllerCustomization();
        customization.Customize(fixture);

        // Act & Assert - Should not throw
        var nonController = fixture.Create<NonControllerClass>();
        Assert.IsNotNull(nonController);
    }

    private class TestController : ControllerBase
    {
        public string TestProperty { get; set; }
    }

    private class NonControllerClass
    {
        public string Name { get; set; }
    }
}