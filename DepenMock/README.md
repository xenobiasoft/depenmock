# DepenMock

A powerful C# unit testing library that automatically mocks your SUT (System Under Test) dependencies. This package contains the core functionality of DepenMock.

## Features

- Automatic dependency mocking
- Seamless integration with popular testing frameworks
- Support for complex dependency chains
- Easy to use API
- Built on top of Moq and AutoFixture

## Installation

```bash
dotnet add package DepenMock
```

## Quick Start

```csharp
// Create a test fixture
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
}
```

## Additional Packages

- [DepenMock.NUnit](https://www.nuget.org/packages/DepenMock.NUnit) - NUnit integration
- [DepenMock.XUnit](https://www.nuget.org/packages/DepenMock.XUnit) - XUnit integration

## Documentation

For detailed documentation, please visit our [GitHub repository](https://github.com/xenobiasoft/depenmock).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
