# DepenMock.NUnit

NUnit integration package for DepenMock, making it easy to use auto-mocking in your NUnit tests.

## Features

- Seamless integration with NUnit
- Automatic dependency mocking
- Support for NUnit's test attributes
- Built on top of DepenMock core

## Installation

```bash
dotnet add package DepenMock.NUnit
```

## Quick Start

```csharp
using NUnit.Framework;
using DepenMock.NUnit;

public class MyTestFixture : AutoMockingTestFixture
{
    [Test]
    public void MyTest()
    {
        // Arrange
        var sut = CreateSut<MyClass>();

        // Act
        var result = sut.DoSomething();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void TestWithMockSetup()
    {
        // Arrange
        var mock = GetMock<IMyInterface>();
        mock.Setup(x => x.GetValue()).Returns("test");

        var sut = CreateSut<MyClass>();

        // Act
        var result = sut.ProcessValue();

        // Assert
        Assert.That(result, Is.EqualTo("test"));
    }
}
```

## Dependencies

- DepenMock (core package)
- NUnit

## Documentation

For detailed documentation, please visit our [GitHub repository](https://github.com/xenobiasoft/depenmock).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
