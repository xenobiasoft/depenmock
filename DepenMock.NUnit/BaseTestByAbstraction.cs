using System;
using DepenMock.Attributes;
using DepenMock.Helpers;
using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace DepenMock.NUnit;

/// <summary>
/// Serves as a base class for tests that rely on abstraction, providing functionality to resolve the system under test
/// (SUT).
/// </summary>
/// <remarks>This class is designed to facilitate testing scenarios where the system under test is resolved from a
/// dependency container. Derived classes can use the <see cref="ResolveSut"/> method to retrieve an instance of
/// <typeparamref name="TTestType"/>.</remarks>
/// <typeparam name="TTestType">The concrete type of the system under test. Must be a class that implements <typeparamref name="TInterfaceType"/>.</typeparam>
/// <typeparam name="TInterfaceType">The interface or base type that <typeparamref name="TTestType"/> implements or inherits.</typeparam>
public abstract class BaseTestByAbstraction<TTestType, TInterfaceType> : BaseTest where TTestType : class, TInterfaceType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestByAbstraction"/> class and registers logger implementations 
    /// in the dependency container.
    /// </summary>
    /// <remarks>This constructor ensures that logger implementations are available for dependency injection 
    /// during the test execution.</remarks>
    protected BaseTestByAbstraction()
    {
        PerformSetup();
    }

    /// <summary>
    /// Sets up the test environment by registering the logger implementation in the dependency container.
    /// </summary>
    /// <remarks>This method is intended to be used as a setup step in unit tests, ensuring that the specified
    /// logger  implementation is available for dependency injection during the test execution.</remarks>
    [SetUp]
    public void Setup()
    {
        PerformSetup();
    }

    /// <summary>
    /// Performs the setup logic, ensuring it only runs once per test instance.
    /// </summary>
    private void PerformSetup()
    {
        Container.Register<ILogger<TTestType>, ListLogger<TTestType>>(Logger);
        Container.Register<ILogger, ListLogger<TTestType>>(Logger);
        AddContainerCustomizations(Container);
    }

    /// <summary>
    /// Resolves an instance of the system under test (SUT) from the container.
    /// </summary>
    /// <remarks>This method attempts to resolve an instance of the specified type from the container.  If the
    /// container is null or the type cannot be resolved, the method returns <see langword="null"/>.</remarks>
    /// <returns>An instance of <typeparamref name="TInterfaceType"/> if successfully resolved; otherwise, <see
    /// langword="null"/>.</returns>
    protected TInterfaceType? ResolveSut() => Container?.Resolve<TTestType>();

    /// <summary>
    /// Gets the logger instance used for logging messages related to the current test type.
    /// </summary>
    public ListLogger<TTestType> Logger { get; } = new();

    /// <summary>
    /// Tears down the test environment and outputs log messages if configured.
    /// </summary>
    /// <remarks>This method is executed after each test to output log messages when the <see cref="LogOutputAttribute"/>
    /// is present on the test method or class. It uses NUnit's TestContext to determine test results and output
    /// log messages accordingly.</remarks>
    [TearDown]
    public void TearDown()
    {
        try
        {
            var testContext = TestContext.CurrentContext;
            var testMethod = GetType().GetMethod(testContext.Test.MethodName);
            var testClass = GetType();

            if (testMethod == null) return;

            var testPassed = testContext.Result.Outcome.Status == TestStatus.Passed;
            
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
            TestContext.WriteLine($"Warning: Failed to output log messages - {ex.Message}");
        }
    }

    /// <summary>
    /// Allows derived classes to add custom ISpecimenBuilder instances to the container's fixture.
    /// </summary>
    /// <param name="container">The test's dependency injection container.</param>
    protected virtual void AddContainerCustomizations(Container container) { }
}