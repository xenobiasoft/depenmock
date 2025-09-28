using System;
using DepenMock.Attributes;
using DepenMock.Helpers;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DepenMock.MSTest;

/// <summary>
/// Provides a base class for test implementations that utilize abstraction, enabling dependency injection and logging
/// functionality for the system under test (SUT).
/// </summary>
/// <remarks>This class serves as a foundation for tests that require resolving the SUT from a dependency
/// injection container and logging operations. It ensures that the SUT is properly constructed with its dependencies
/// and provides a logger instance for test-related logging.</remarks>
/// <typeparam name="TTestType">The concrete type of the system under test (SUT).</typeparam>
/// <typeparam name="TInterfaceType">The interface type implemented by <typeparamref name="TTestType"/>.</typeparam>
public abstract class BaseTestByAbstraction<TTestType, TInterfaceType> : BaseTest where TTestType : class, TInterfaceType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestByAbstraction"/> class.
    /// </summary>
    /// <remarks>This constructor registers a logger implementation for the specified interface type in the
    /// dependency injection container. The logger is configured using the provided <see cref="Logger"/>
    /// instance.</remarks>
    protected BaseTestByAbstraction()
    {
        Container.Register<ILogger<TTestType>, ListLogger<TTestType>>(Logger);
        Container.Register<ILogger, ListLogger<TTestType>>(Logger);
        AddContainerCustomizations(Container);
    }

    /// <summary>
    /// Allows derived classes to add custom ISpecimenBuilder instances to the container's fixture.
    /// </summary>
    /// <param name="container">The test's dependency injection container.</param>
    protected virtual void AddContainerCustomizations(Container container) { }

    /// <summary>
    /// Resolves an instance of the system under test (SUT) from the dependency injection container.
    /// </summary>
    /// <remarks>This method retrieves an instance of the specified type <typeparamref
    /// name="TTestType"/> from the container,  ensuring that the SUT is properly constructed with its dependencies.
    /// If the container is null, the method  returns null.</remarks>
    /// <returns>An instance of <typeparamref name="TTestType"/> if the container is available; otherwise, null.</returns>
    protected TInterfaceType ResolveSut() => Container?.Resolve<TTestType>();

    /// <summary>
    /// Gets the logger instance used for logging operations.
    /// </summary>
    public ListLogger<TTestType> Logger { get; } = new();

    /// <summary>
    /// Gets or sets the test context which provides information about and functionality for the current test run.
    /// </summary>
    public TestContext? TestContext { get; set; }

    /// <summary>
    /// Initializes the test environment before each test.
    /// </summary>
    /// <remarks>This method is executed before each test to clear any previous log messages,
    /// ensuring log isolation between tests.</remarks>
    [TestInitialize]
    public void Initialize()
    {
        Logger.Clear(); // Clear logs from any previous tests
    }

    /// <summary>
    /// Cleans up after each test and outputs log messages if configured.
    /// </summary>
    /// <remarks>This method is executed after each test to output log messages when the <see cref="LogOutputAttribute"/>
    /// is present on the test method or class. It uses MSTest's TestContext to determine test results and output
    /// log messages accordingly.</remarks>
    [TestCleanup]
    public void CleanUp()
    {
        try
        {
            if (TestContext == null)
                return;

            var testMethod = GetType().GetMethod(TestContext.TestName);
            var testClass = GetType();

            if (testMethod == null)
                return;

            var testPassed = TestContext.CurrentTestOutcome == UnitTestOutcome.Passed;
            
            if (LogOutputHelper.ShouldOutputLogs(testMethod, testClass, testPassed))
            {
                var logOutput = LogOutputHelper.FormatLogMessages(Logger);
                if (!string.IsNullOrWhiteSpace(logOutput))
                {
                    TestContext.WriteLine(logOutput);
                }
            }
        }
        catch (Exception ex)
        {
            // Don't let log output failures break the test
            TestContext?.WriteLine($"Warning: Failed to output log messages - {ex.Message}");
        }
    }
}