using AutoFixture;
using AutoFixture.AutoMoq;
using DepenMock.Customizations;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Tests.NUnit;

[TestFixture]
public class ControllerCustomizationTests
{
    [Test]
    public void Customize_ShouldAddControllerSpecification()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var customization = new ControllerCustomization();

        // Act & Assert - Should not throw
        Assert.DoesNotThrow(() => customization.Customize(fixture));
        Assert.That(customization, Is.Not.Null);
    }

    [Test]
    public void Customize_WithControllerType_ShouldCreateControllerWithHttpContext()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var customization = new ControllerCustomization();
        customization.Customize(fixture);

        // Act
        var controller = fixture.Create<TestController>();

        // Assert
        Assert.That(controller, Is.Not.Null);
        Assert.That(controller.ControllerContext, Is.Not.Null);
        Assert.That(controller.ControllerContext.HttpContext, Is.Not.Null);
    }

    [Test]
    public void Customize_WithNonControllerType_ShouldWorkNormally()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var customization = new ControllerCustomization();
        customization.Customize(fixture);

        // Act & Assert - Should not throw
        var nonController = fixture.Create<NonControllerClass>();
        Assert.That(nonController, Is.Not.Null);
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