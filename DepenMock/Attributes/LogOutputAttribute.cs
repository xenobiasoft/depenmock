using System;

namespace DepenMock.Attributes;

/// <summary>
/// Specifies when log messages should be output to the test runner's output window.
/// </summary>
public enum LogOutputTiming
{
    /// <summary>
    /// Always output log messages regardless of test result.
    /// </summary>
    Always,

    /// <summary>
    /// Output log messages only when the test fails.
    /// </summary>
    OnFailure,

    /// <summary>
    /// Output log messages only when the test succeeds.
    /// </summary>
    OnSuccess
}

/// <summary>
/// Attribute that can be applied to test methods or test classes to control when
/// log messages should be output to the test runner's output window.
/// </summary>
/// <remarks>
/// This attribute configures when the logged messages captured by the test logger
/// should be displayed in the test runner output. The timing is controlled by the
/// <see cref="Timing"/> property.
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class LogOutputAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogOutputAttribute"/> class
    /// with the default timing of <see cref="LogOutputTiming.Always"/>.
    /// </summary>
    public LogOutputAttribute() : this(LogOutputTiming.Always)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogOutputAttribute"/> class
    /// with the specified timing.
    /// </summary>
    /// <param name="timing">The timing that determines when log messages should be output.</param>
    public LogOutputAttribute(LogOutputTiming timing)
    {
        Timing = timing;
    }

    /// <summary>
    /// Gets the timing that determines when log messages should be output.
    /// </summary>
    public LogOutputTiming Timing { get; }
}