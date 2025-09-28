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

## Framework Support and Limitations

- **NUnit**: Full support using `TestContext.WriteLine()`
- **MSTest**: Full support using `TestContext.WriteLine()`
- **xUnit v2**: Limited support due to framework constraints. The attribute works but test result detection is simplified.
- **xUnit v3**: Enhanced support with better test lifecycle integration.

## xUnit Specific Notes

For xUnit tests, the `LogOutputAttribute` has some limitations in v2:

1. Test result detection is simplified (assumes tests pass if no exception is thrown)
2. Log output timing may not work exactly as expected in all scenarios

### xUnit v3 Improvements

When using xUnit v3, you'll get:
- Better test result detection
- More accurate timing control
- Improved integration with the test framework

## Migration Path

Your library will work with both xUnit v2 and v3. Users can:

1. **Stay on xUnit v2**: LogOutput will work with limitations noted above
2. **Upgrade to xUnit v3**: LogOutput will work with full functionality

No code changes are required in your test classes when upgrading from v2 to v3.

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