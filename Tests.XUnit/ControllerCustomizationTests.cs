using AutoFixture;
using AutoFixture.AutoMoq;
using DepenMock.Customizations;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Tests.XUnit;

public class ControllerCustomizationTests
{
    [Fact]
    public void Customize_ShouldAddControllerSpecification()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var customization = new ControllerCustomization();

        // Act
        var exception = Record.Exception(() => customization.Customize(fixture));

        // Assert
        Assert.Null(exception);
        Assert.NotNull(customization);
    }

    [Fact]
    public void Customize_WithControllerType_ShouldCreateControllerWithHttpContext()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var customization = new ControllerCustomization();
        customization.Customize(fixture);

        // Act
        var controller = fixture.Create<TestController>();

        // Assert
        Assert.NotNull(controller);
        Assert.NotNull(controller.ControllerContext);
        Assert.NotNull(controller.ControllerContext.HttpContext);
    }

    [Fact]
    public void Customize_WithNonControllerType_ShouldWorkNormally()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var customization = new ControllerCustomization();
        customization.Customize(fixture);

        // Act & Assert - Should not throw
        var nonController = fixture.Create<NonControllerClass>();
        Assert.NotNull(nonController);
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