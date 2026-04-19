# DepenMock.XUnit

XUnit integration for [DepenMock](https://github.com/xenobiasoft/depenmock) — a C# testing library that automates mock creation for your System Under Test (SUT) dependencies.

## Installation

Install this package alongside a DepenMock mock-framework package:

```
dotnet add package DepenMock.XUnit
dotnet add package DepenMock.Moq          # or DepenMock.NSubstitute
```

## Setting Up the Test Container

The base classes require an `IMockFactory` to be passed through the constructor. The recommended approach is to define a project-level base class once that wires up your chosen mock factory, so individual test classes need no constructor boilerplate.

**Recommended: project-level base class**

Define these once in your test project and inherit from them everywhere:

```c#
public abstract class TestByAbstraction<TType, TInterface>
    : BaseTestByAbstraction<TType, TInterface>(new MoqMockFactory())
    where TType : class, TInterface;

public abstract class TestByType<TType>
    : BaseTestByType<TType>(new MoqMockFactory())
    where TType : class;
```

Then each test class simply inherits with no constructor required:

```c#
public class DeskBookingRequestProcessorTests
    : TestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
    // Your test methods here
}
```

**Alternative: pass the factory directly**

If you prefer not to define a shared base class, pass the factory explicitly per test class:

```c#
public class DeskBookingRequestProcessorTests
    : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
    public DeskBookingRequestProcessorTests()
        : base(new MoqMockFactory()) { }
}

public class AccountControllerTests : BaseTestByType<AccountController>
{
    public AccountControllerTests()
        : base(new MoqMockFactory()) { }
}
```

Swap `MoqMockFactory` for `NSubstituteMockFactory` to use NSubstitute instead.

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

DepenMock automatically creates mocks for all unregistered dependencies using the factory you provided. Call `ResolveMock<T>()` to get a reference to a mock, then unwrap it with the extension method for your chosen framework (`AsMoq()` or `AsNSubstitute()`) to access stub and spy APIs.

See [DepenMock.Moq](https://github.com/xenobiasoft/depenmock/tree/main/DepenMock.Moq) or [DepenMock.NSubstitute](https://github.com/xenobiasoft/depenmock/tree/main/DepenMock.NSubstitute) for framework-specific stub and spy examples.

## Testing Logging

DepenMock provides a `ListLogger` and automatically injects it as a dependency for your tests.

```c#
Logger.ErrorLogs().AssertContains($"Correlation Id: {correlationId}");

// Accessing logs by level directly
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
