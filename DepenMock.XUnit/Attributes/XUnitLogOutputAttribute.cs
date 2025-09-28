using DepenMock.Attributes;
using System;
using System.Reflection;
using Xunit.Sdk;

namespace DepenMock.XUnit.Attributes;

/// <summary>
/// xUnit-specific log output attribute that provides better integration with xUnit v3 while maintaining v2 compatibility.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class XUnitLogOutputAttribute : BeforeAfterTestAttribute
{
    private readonly LogOutputTiming _timing;

    /// <summary>
    /// Initializes a new instance of the <see cref="XUnitLogOutputAttribute"/> class.
    /// </summary>
    /// <param name="timing">When to output log messages.</param>
    public XUnitLogOutputAttribute(LogOutputTiming timing = LogOutputTiming.OnFailure)
    {
        _timing = timing;
    }

    /// <summary>
    /// Called after the test method is executed.
    /// </summary>
    /// <param name="methodUnderTest">The method under test.</param>
    public override void After(MethodInfo methodUnderTest)
    {
        // This implementation will work better in xUnit v3
        // For v2, functionality is limited due to framework constraints
    }
}