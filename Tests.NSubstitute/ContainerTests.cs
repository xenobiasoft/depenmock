using DepenMock.NSubstitute;
using NSubstitute;

namespace Tests.NSubstitute;

public class ContainerTests
{
    private readonly DepenMock.Container _container;

    public ContainerTests()
    {
        _container = new DepenMock.Container(new NSubstituteMockFactory());
    }

    [Fact]
    public void SetupMock_ShouldApplyConfigurationToResolvedMock()
    {
        // Assemble
        var expectedValue = _container.Create<string>();

        // Act
        _container.SetupMock<ITestService>(s => s.GetValue().Returns(expectedValue));

        // Assert
        Assert.Equal(expectedValue, _container.ResolveMock<ITestService>().Object.GetValue());
    }

    [Fact]
    public void SetupMock_ShouldReturnSameContainerForChaining()
    {
        // Act
        var result = _container.SetupMock<ITestService>(s => s.GetValue().Returns("value"));

        // Assert
        Assert.Same(_container, result);
    }

    [Fact]
    public void SetupMock_ShouldConfigureThePreviouslyResolvedMock()
    {
        // Assemble
        var expectedValue = _container.Create<string>();
        var mock = _container.ResolveMock<ITestService>();

        // Act
        _container.SetupMock<ITestService>(s => s.GetValue().Returns(expectedValue));

        // Assert
        Assert.Same(mock, _container.ResolveMock<ITestService>());
        Assert.Equal(expectedValue, mock.Object.GetValue());
    }

    [Fact]
    public void SetupMock_WithNullSetup_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _container.SetupMock<ITestService>(null));
    }

    public interface ITestService
    {
        string GetValue();
    }
}
