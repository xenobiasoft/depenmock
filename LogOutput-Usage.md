# LogOutput Attribute Usage

The `LogOutputAttribute` allows you to control when logged messages should be displayed in your test runner's output window.

## Basic Usage

### Method Level

```csharp
[Test]
[LogOutput(LogOutputTiming.Always)]
public void MyTest()
{
    var service = ResolveSut();
    service.DoSomethingThatLogs();
    
    // Logs will always be displayed in the test output
}
```

### Class Level

```csharp
[TestFixture]
[LogOutput(LogOutputTiming.OnFailure)]
public class MyTestClass : BaseTestByType<MyService>
{
    // All tests in this class will show logs only when they fail
}
```

## LogOutputTiming Options

- **`Always`**: Output log messages regardless of test result
- **`OnSuccess`**: Output log messages only when the test passes
- **`OnFailure`**: Output log messages only when the test fails

## Framework Support

The LogOutput attribute works with the following testing frameworks:
- **NUnit**: Uses `TestContext.WriteLine()` - ✅ Fully supported
- **MSTest**: Uses `TestContext.WriteLine()` - ✅ Fully supported  
- **xUnit v3**: Uses `ITestOutputHelper.WriteLine()` - ✅ Fully supported (use `DepenMock.XUnit.V3.Attributes.LogOutputAttribute`)
- **xUnit v2**: ❌ Not supported - xUnit v2 does not provide test outcome information needed to conditionally output logs

### xUnit Version Differences

**xUnit v3 (DepenMock.XUnit.V3)**
```csharp
using DepenMock.XUnit.V3.Attributes;

[Fact]
[LogOutput(LogOutputTiming.OnFailure)]
public void MyTest()
{
    // Logs will be output only if the test fails
}
```

**xUnit v2 (DepenMock.XUnit)**
```csharp
// LogOutputAttribute is not supported in xUnit v2
// Use Logger.Logs directly to inspect log messages in assertions
[Fact]
public void MyTest()
{
    var service = ResolveSut();
    service.DoWork();
    
    // Manually check logs in assertions
    Assert.Contains("Expected message", Logger.Logs[LogLevel.Information]);
}
```

The reason xUnit v2 doesn't support LogOutputAttribute is that xUnit v2 doesn't provide access to test execution outcomes (pass/fail) during the test lifecycle. xUnit v3 added this capability through the `IBeforeAfterTestAttribute` interface and `TestContext.TestState` property.

## Example Output

When a test with `[LogOutput(LogOutputTiming.Always)]` runs, you'll see:

```
=== Test Log Messages ===
[Information]
  Processing: TestMessage
[Warning]
  This is a warning message
[Error]
  An error occurred: Details here
```