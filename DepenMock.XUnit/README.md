# DepenMock.XUnit

XUnit integration package for DepenMock, making it easy to use auto-mocking in your XUnit tests.

## Features

- Seamless integration with XUnit
- Automatic dependency mocking
- Support for XUnit's test attributes
- Built on top of DepenMock core

## Installation

```bash
dotnet add package DepenMock.XUnit
```

## Quick Start

```csharp
using Xunit;
using DepenMock.XUnit;

public class MyTestFixture : AutoMockingTestFixture
{
    [Fact]
    public void MyTest()
    {
        // Arrange
        var sut = CreateSut<MyClass>();

        // Act
        var result = sut.DoSomething();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void TestWithMockSetup()
    {
        // Arrange
        var mock = GetMock<IMyInterface>();
        mock.Setup(x => x.GetValue()).Returns("test");

        var sut = CreateSut<MyClass>();

        // Act
        var result = sut.ProcessValue();

        // Assert
        Assert.Equal("test", result);
    }
}
```

## Dependencies

- DepenMock (core package)
- xUnit

## Documentation

For detailed documentation, please visit our [GitHub repository](https://github.com/xenobiasoft/depenmock).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
