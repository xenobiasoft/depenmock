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

The LogOutput attribute works with all supported testing frameworks:
- **NUnit**: Uses `TestContext.WriteLine()`
- **MSTest**: Uses `TestContext.WriteLine()`
- **xUnit**: Uses `ITestOutputHelper.WriteLine()`

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