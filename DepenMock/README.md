# DepenMock - C# Testing Library

## Introduction

DepenMock is a robust C# testing library designed to streamline the mocking process for your System Under Test (SUT) dependencies. By automating the mock creation process, DepenMock simplifies unit testing and encourages adherence to established software design principles.

## Setting Up the Test Container

To effectively utilize DepenMock in your unit tests, consider the following base classes that your test classes should inherit from:

**BaseTestByAbstraction**

This base class is suitable for testing your SUT when it implements an interface. Testing through interfaces helps ensure adherence to the Liskov Substitution Principle (LSP).

```c#
public class DeskBookingRequestProcessorTests : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
    // Your test methods here
}
```

**BaseTestByType**

Use this base class when your SUT does not implement an interface, such as when testing an API controller.

```c#
public class AccountControllerTests : BaseTestByType<AccountController>
{
    // Your test methods here
}
```

## Creating the System Under Test (SUT)

To create an instance of your SUT, use the `ResolveSut` method. It is essential to set up any dependencies that need to be mocked before creating the SUT. Place the ResolveSut call as the final step before executing your test action.

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

The `CreateMany<T>` method generates a list of instances of the specified type. By default, it creates a list of three items, but you can customize the number of instances by setting by setting the `numberOfInstances` parameter.

```c#
// Creates a list of strings
var randomStrings = Container.CreateMany<string>();

// Creates a list of random integers
var randomNumbers = Containers.CreateMany<int>();

// Creates a list of boolean values. All values will be set to true.
var allTrueValues = Container.CreateMany<bool>();

// Creates a list of random dates. This example creates a list of 5 random dates.
var randomDates = Container.CreateMany<DateTime>(5);

// Creates a list of addresses set to random values
var addressList = Container.CreateMany<Address>();
```

**Build\<T>()**

The `Build<T>` method provides a builder pattern for creating objects with specified data rather than using all generated data.

```c#
var deskBookingResult = Container
    .Build<DeskBookingResult>()
    .Without(x => x.DeskBookingId) // Here, DeskBookingId will be set to null
    .With(x => x.Code, DeskBookingResultCode.Success)
    .With(x => x.FirstName, request.FirstName)
    .With(x => x.LastName, request.LastName)
    .With(x => x.Email, request.Email)
    .With(x => x.Date, request.Date)
    .Create();
```

## Creating Mock Dependencies

### Registering Mocked Dependencies

DepenMock's testing framework automatically creates mock dependencies, but you can obtain a reference to a mock dependency to set up methods to return predefined data (stub) or to verify interactions with the dependency (spy).

**Creating a stub**

```c#
Container
    .ResolveMock<IDeskRepository>()
    .Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
    .Returns(Container.CreateMany<Desk>());
```

**Creating a spy**

```c#
var mockRepo = Container.ResolveMock<IDeskBookingRepository>();

mockRepo.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);
```

## Testing Logging

DepenMock provides a logger object and automatically injects it as a dependency for your tests.

```c#
Assert.That(Logger.Logs[LogLevel.Error].TrueForAll(x => x.Contains($"Correlation Id: {correlationId}")));

// Checking different levels of error logs
// Error
Logger.Logs[LogLevel.Error]

// Critical
Logger.Logs[LogLevel.Critical]

// Warning
Logger.Logs[LogLevel.Warning]

// Information
Logger.Logs[LogLevel.Information]
```

## Extending Fixture Customization

DepenMock allows you to add your own customizations to the underlying AutoFixture fixture by overriding a virtual method in your test class. This is useful if you want to add custom `ISpecimenBuilder` instances or otherwise control how objects are created.

### How to Add Custom ISpecimenBuilder Instances

Override the `AddContainerCustomizations` method in your test class and use the `Container.AddCustomizations` method to add your custom builders:

```c#
public class MyTests : BaseTestByType<MyType>
{
    protected override void AddContainerCustomizations(Container container)
    {
        container.AddCustomizations(new MyCustomSpecimenBuilder());
    }
}
```

This works for all base test types (`BaseTestByType`, `BaseTestByAbstraction`, and their NUnit/XUnit/MSTest variants). The method is called automatically during test setup.

You can add as many custom builders as you need:

```c#
container.AddCustomizations(new MyBuilder1(), new MyBuilder2());
```

See the API docs for more details on `ISpecimenBuilder` and advanced customization scenarios.

## Sample Project

Explore the sample project, DeskBooker.Core, which includes example unit tests for NUnit, XUnit, and MSTest to help you get started with DepenMock.
