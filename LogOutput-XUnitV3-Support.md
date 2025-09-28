# LogOutput Attribute - xUnit v3 Support

The `LogOutputAttribute` now supports proper test outcome detection in xUnit v3, while maintaining full backward compatibility with xUnit v2.

## How It Works

### xUnit v2 Behavior (Existing)
- Uses the disposal pattern to detect when tests complete
- Assumes tests passed if no exception was thrown during disposal
- Limited ability to detect actual test outcomes

### xUnit v3 Behavior (New)
- Uses xUnit v3's `TestContextAccessor` and `ITestContext` to get actual test results
- Properly detects when tests pass, fail, or are skipped
- Provides accurate log output based on the configured `LogOutputTiming`

## Usage

The usage remains exactly the same regardless of xUnit version:

```csharp
[Fact]
[LogOutput(LogOutputTiming.OnFailure)]
public void MyTest_ShouldLogOnFailure()
{
    var service = ResolveSut();
    service.DoWork("This will only appear if the test fails");
    
    // Test logic here
}

[Fact]  
[LogOutput(LogOutputTiming.OnSuccess)]
public void MyTest_ShouldLogOnSuccess()
{
    var service = ResolveSut();
    service.DoWork("This will only appear if the test passes");
    
    // Test logic here
}

[Fact]
[LogOutput(LogOutputTiming.Always)]
public void MyTest_ShouldAlwaysLog()
{
    var service = ResolveSut();
    service.DoWork("This will always appear regardless of test outcome");
    
    // Test logic here  
}
```

## LogOutputTiming Options

- **`Always`**: Log messages are always output regardless of test outcome
- **`OnFailure`**: Log messages are only output when the test fails
- **`OnSuccess`**: Log messages are only output when the test passes

## Version Detection

The library automatically detects which version of xUnit is being used:

- If xUnit v3 is available, it uses the enhanced test context to get actual test outcomes
- If xUnit v2 is being used, it falls back to the previous behavior
- No configuration or code changes required

## Migration from xUnit v2 to v3

1. Update your xUnit packages to v3
2. No code changes required in your tests
3. The LogOutput attribute will automatically use the enhanced xUnit v3 features
4. You'll now get accurate log output based on actual test outcomes

## Technical Details

The implementation uses reflection to detect xUnit v3 availability and access the test context APIs:

- `XUnitV3Helper.IsXUnitV3Available` - detects if xUnit v3 is loaded
- `XUnitV3Helper.TryGetTestResult()` - attempts to get the current test result from xUnit v3 context
- Falls back gracefully to xUnit v2 behavior when v3 is not available

This approach ensures that the same DepenMock.XUnit package works with both xUnit v2 and v3 without requiring separate packages or conditional compilation.