# DepenMock
A C# testing library that will automatically mock your SUT (system under test) dependencies.

## Setting Up the Test Container

There are a couple base classes your unit tests should inherit from.

**BaseTestByAbstraction**

This base class will test your sut using the implemented interface. You should use this base class if your sut implements an interface. Testing by an interface allows you to better ensure your classes conform to Liskov's Substitution Principle.

```c#
public class DeskBookingRequestProcessorTests : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
    ...
}
```

**BaseTestByType**

You should use this base class when you're sut does not implement an interface, e.g. testing an API controller.

```c#
public class AccountControllerTests : BaseTestByType<AccountController>
{
    ...
}
```

## Creating the **sut**

Use the method ```ResolveSut``` to create an instance of your sut.

NOTE: Any dependencies you need to mock needs to be set up before creating the sut. Using ```ResolveSut``` should be the **last** thing in your test before executing the Action.

```c#
var sut = ResolveSut();
```

## Creating mock data

**Create\<T>()**

```Create<T>``` creates a single instance of the type you want to create.

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

```CreateMany<T>``` creates a list of the instance type specified. By default, creates a list of 3 items. You can change the number of instances created by setting ```numberOfInstances```.

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

```Build<T>``` provides a builder pattern for creating objects with specified data instead of all generated data.

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

### Registering Mocked Dependencies

The testing framework will automatically create mock dependencies for you. But you can get a reference to a mock dependency so you can set up methods to return canned data (a stub), or if you need to verify interactions with the dependency (a spy).

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

The framework provides a logger object and automatically injects it as a dependency.

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

## Sample Project

There is a sample project, DeskBooker.Core, that has example unit tests for NUnit and XUnit.