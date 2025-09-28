using DepenMock.XUnit.Internal;
using Xunit;

namespace Tests.XUnit;

/// <summary>
/// Tests for the XUnitV3Helper functionality to ensure proper version detection and test result extraction.
/// </summary>
public class XUnitV3HelperTests
{
    [Fact]
    public void IsXUnitV3Available_InXUnitV2Environment_ShouldReturnFalse()
    {
        // Act
        var isAvailable = XUnitV3Helper.IsXUnitV3Available;
        
        // Assert
        // In our current test environment (xUnit v2), this should return false
        Assert.False(isAvailable);
    }

    [Fact]
    public void TryGetTestResult_InXUnitV2Environment_ShouldReturnUnavailable()
    {
        // Act
        var (isAvailable, testPassed) = XUnitV3Helper.TryGetTestResult();
        
        // Assert
        // In xUnit v2 environment, should return false for isAvailable
        Assert.False(isAvailable);
        // testPassed value doesn't matter when isAvailable is false
    }
    
    [Fact]
    public void TryGetTestResult_CalledMultipleTimes_ShouldBeConsistent()
    {
        // Act
        var result1 = XUnitV3Helper.TryGetTestResult();
        var result2 = XUnitV3Helper.TryGetTestResult();
        
        // Assert
        Assert.Equal(result1.IsAvailable, result2.IsAvailable);
        // Note: testPassed might vary if the test context changes, but isAvailable should be consistent
    }
}