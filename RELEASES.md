# Release Notes

---

## v3.0.0

### What's Changed

**NSubstitute support added (`DepenMock.NSubstitute`)**

A new `DepenMock.NSubstitute` package is now available, giving teams the option to use NSubstitute as their mocking framework instead of Moq. It includes `NSubstituteMockFactory`, `NSubstituteMock<T>`, and a set of `NSubstituteExtensions` for unwrapping substitutes from the `IMock<T>` abstraction.

```csharp
// Stub using NSubstitute
Container.ResolveMock<IDeskRepository>().AsNSubstitute()
    .GetAvailableDesks(Arg.Any<DateTime>())
    .Returns(Container.CreateMany<Desk>());

// Spy using NSubstitute
Container.ResolveMock<IDeskBookingRepository>().AsNSubstitute()
    .Received(1).Save(Arg.Any<DeskBooking>());
```

**Mock factory is now explicit on all base classes**

The base classes `BaseTestByAbstraction<TTestType, TInterfaceType>`, `BaseTestByType<TTestType>`, and `BaseTest` now require an `IMockFactory` to be passed explicitly to their constructors. This replaces the previous behavior where `MoqMockFactory` was used internally by default. This change makes the choice of mocking framework visible and consistent at every level of the hierarchy.

### Migration Guide

**Updating constructor calls**

All test classes that inherit from a DepenMock base class must now forward an `IMockFactory` to the base constructor. Choose the factory that matches your mocking framework.

For Moq users (add a reference to `DepenMock.Moq` if not already present):

```csharp
// Before (v2.0.0)
public class DeskBookingRequestProcessorTests
    : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
    // no constructor needed
}

// After (v3.0.0)
public class DeskBookingRequestProcessorTests
    : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
    public DeskBookingRequestProcessorTests() : base(new MoqMockFactory()) { }
}
```

For NSubstitute users (add a reference to `DepenMock.NSubstitute`):

```csharp
public class DeskBookingRequestProcessorTests
    : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
    public DeskBookingRequestProcessorTests() : base(new NSubstituteMockFactory()) { }
}
```

The same pattern applies to `BaseTestByType<T>` and any custom class that extends `BaseTest` directly.

### Breaking Changes

- `BaseTest`, `BaseTestByAbstraction<TTestType, TInterfaceType>`, and `BaseTestByType<TTestType>` constructors now require an `IMockFactory` parameter. Any test class that previously relied on the parameterless base constructor will fail to compile and must be updated to pass a mock factory explicitly.

---

## v2.0.0

### What's Changed

**Mocking framework abstracted into `DepenMock.Moq`**

Moq is no longer bundled directly inside the core `DepenMock` package. Mocking is now handled through two new interfaces — `IMock<T>` and `IMockFactory` — and a dedicated `DepenMock.Moq` package that provides the concrete Moq implementation (`MoqMockFactory`, `MoqMock<T>`, and `MoqExtensions`). This separation allows alternative mocking frameworks to be supported in future releases.

**`ResolveMock<T>()` returns `IMock<T>`**

The return type of `Container.ResolveMock<T>()` has changed from `Mock<T>` to `IMock<T>`. To access Moq-specific APIs such as `Setup` and `Verify`, call the `.AsMoq()` extension method provided by `DepenMock.Moq`.

```csharp
// Stub
Container.ResolveMock<IDeskRepository>().AsMoq()
    .Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
    .Returns(Container.CreateMany<Desk>());

// Spy
var mockRepo = Container.ResolveMock<IDeskBookingRepository>().AsMoq();
mockRepo.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);
```

**`DepenMock.XUnit.V3` added to release pipeline**

The `DepenMock.XUnit.V3` package is now included in the standard release and publish pipeline alongside the other framework-specific packages.

**Bug fixes and enhancements**

- Fixed `ILogger.IsEnabled` to return `true` for all log levels except `LogLevel.None`.
- Improved `BaseTestByAbstraction<TTestType, TInterfaceType>` in the xUnit package with several minor enhancements.

### Migration Guide

**Add the `DepenMock.Moq` package**

Add a reference to the new `DepenMock.Moq` NuGet package alongside your existing framework-specific package (e.g., `DepenMock.XUnit`, `DepenMock.NUnit`, or `DepenMock.MSTest`).

**Update `ResolveMock<T>()` call sites**

Any code that uses the result of `Container.ResolveMock<T>()` directly with Moq APIs must be updated to call `.AsMoq()` first.

```csharp
// Before (v1.x)
Container.ResolveMock<IDeskRepository>()
    .Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
    .Returns(Container.CreateMany<Desk>());

// After (v2.0.0)
Container.ResolveMock<IDeskRepository>().AsMoq()
    .Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
    .Returns(Container.CreateMany<Desk>());
```

**Updating direct `Container` instantiation**

If you instantiate `Container` directly (outside of a base class), you must now pass an `IMockFactory` to the constructor.

```csharp
// Before (v1.x)
var container = new Container();

// After (v2.0.0)
var container = new Container(new MoqMockFactory());
```

Test classes that inherit from a DepenMock base class do not need a constructor change — the base classes handle factory creation internally in v2.0.0.

### Breaking Changes

- `Container.ResolveMock<T>()` now returns `IMock<T>` instead of `Mock<T>`. Any call site that invokes Moq APIs directly on the return value will fail to compile. Add `.AsMoq()` to restore access to `Setup`, `Verify`, and other Moq members.
- `Container` constructor now requires an `IMockFactory` argument. Direct instantiation of `Container()` without arguments will no longer compile.
- `DepenMock.Moq` is now a required separate NuGet package for Moq support and must be added to your project explicitly.
