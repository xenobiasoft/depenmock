# DepenMock.Moq

Moq integration for [DepenMock](https://github.com/xenobiasoft/depenmock) — a C# testing library that automates mock creation for your System Under Test (SUT) dependencies.

Install this package alongside one of the DepenMock test-framework packages (`DepenMock.XUnit`, `DepenMock.NUnit`, or `DepenMock.MSTest`) to use Moq as your mocking framework.

## Installation

```
dotnet add package DepenMock.Moq
dotnet add package DepenMock.XUnit   # or DepenMock.NUnit / DepenMock.MSTest
```

## Setting Up the Test Container

Pass `MoqMockFactory` to the base class constructor to configure DepenMock to use Moq for all auto-created mocks.

**BaseTestByAbstraction**

Use this base class when your SUT implements an interface. Testing through interfaces helps ensure adherence to the Liskov Substitution Principle (LSP).

```c#
public class DeskBookingRequestProcessorTests
    : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
    public DeskBookingRequestProcessorTests()
        : base(new MoqMockFactory()) { }

    // Your test methods here
}
```

**BaseTestByType**

Use this base class when your SUT does not implement an interface, such as when testing an API controller.

```c#
public class AccountControllerTests : BaseTestByType<AccountController>
{
    public AccountControllerTests()
        : base(new MoqMockFactory()) { }

    // Your test methods here
}
```

## Creating the System Under Test (SUT)

Call `ResolveSut()` as the final step of your arrange phase, after any mock setup.

```c#
var sut = ResolveSut();
```

## Creating Mock Data

**Create\<T>()**

The `Create<T>` method generates a single instance of the specified type.

```c#
// Creates a random string in the format of a guid i.e "fe06998d-aec1-4808-8968-d8f37024a294"
var name = Container.Create<string>();

// Creates a random integer greater than 0
var id = Container.Create<int>();

// Creates a boolean value set to true
var isEnabled = Container.Create<bool>();

// Creates a random date set in the future
var startDate = Container.Create<DateTime>();

// Creates an instance of the class MyObj
var myObjInstance = Container.Create<MyObj>();

// Creates a list of strings. NOTE: Use CreateMany for generating lists.
var randomStrings = Container.Create<List<string>>();
```

**CreateMany\<T>(int? numberOfInstances)**

The `CreateMany<T>` method generates a list of instances of the specified type. By default it creates three items, but you can customise the count with the `numberOfInstances` parameter.

```c#
// Creates a list of strings
var randomStrings = Container.CreateMany<string>();

// Creates a list of random integers
var randomNumbers = Container.CreateMany<int>();

// Creates a list of boolean values. All values will be set to true.
var allTrueValues = Container.CreateMany<bool>();

// Creates a list of random dates. This example creates a list of 5 random dates.
var randomDates = Container.CreateMany<DateTime>(5);

// Creates a list of addresses set to random values
var addressList = Container.CreateMany<Address>();
```

**Build\<T>()**

The `Build<T>` method provides a builder pattern for creating objects with specific values rather than using all auto-generated data.

```c#
var deskBookingResult = Container
    .Build<DeskBookingResult>()
    .Without(x => x.DeskBookingId) // DeskBookingId will be set to null
    .With(x => x.Code, DeskBookingResultCode.Success)
    .With(x => x.FirstName, request.FirstName)
    .With(x => x.LastName, request.LastName)
    .With(x => x.Email, request.Email)
    .With(x => x.Date, request.Date)
    .Create();
```

## Creating Mock Dependencies

DepenMock automatically creates Moq mocks for all unregistered dependencies. Call `ResolveMock<T>()` to get a reference to a mock, then use `AsMoq()` to access Moq's `Setup` and `Verify` APIs.

**Creating a stub**

```c#
Container
    .ResolveMock<IDeskRepository>()
    .AsMoq()
    .Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
    .Returns(Container.CreateMany<Desk>());
```

**Creating a spy**

```c#
Container
    .ResolveMock<IDeskBookingRepository>()
    .AsMoq()
    .Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);
```

## Testing Logging

DepenMock provides a `ListLogger` and automatically injects it as a dependency for your tests.

```c#
Assert.That(Logger.Logs[LogLevel.Error].TrueForAll(x => x.Contains($"Correlation Id: {correlationId}")));

// Checking different log levels
Logger.Logs[LogLevel.Error]
Logger.Logs[LogLevel.Critical]
Logger.Logs[LogLevel.Warning]
Logger.Logs[LogLevel.Information]
```

## Extending Fixture Customization

Override `AddContainerCustomizations` to register custom `ISpecimenBuilder` instances or otherwise control how objects are created.

```c#
public class MyTests : BaseTestByType<MyType>
{
    public MyTests() : base(new MoqMockFactory()) { }

    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new MyCustomSpecimenBuilder());
    }
}
```

You can add as many custom builders as you need:

```c#
container.AddCustomizations(new MyBuilder1(), new MyBuilder2());
```

## Sample Project

Explore the sample project, DeskBooker.Core, which includes example unit tests for NUnit, XUnit, and MSTest to help you get started with DepenMock.
