using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests.MSTest
{
    [TestClass]
    public class ContainerTests
    {
        private DepenMock.Container _container;

        [TestInitialize]
        public void Setup()
        {
            _container = new DepenMock.Container();
        }

        [TestMethod]
        public void Create_WithPrimitiveType_ShouldReturnValue()
        {
            // Act
            var result = _container.Create<int>();

            // Assert
            Assert.IsTrue(result != 0);
        }

        [TestMethod]
        public void Create_WithString_ShouldReturnNonNullString()
        {
            // Act
            var result = _container.Create<string>();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        public void Create_WithComplexType_ShouldReturnInstanceWithPopulatedProperties()
        {
            // Act
            var result = _container.Create<TestModel>();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Name);
            Assert.IsTrue(result.Id != 0);
        }

        [TestMethod]
        public void CreateMany_WithDefaultCount_ShouldReturnThreeItems()
        {
            // Act
            var results = _container.CreateMany<int>();

            // Assert
            Assert.AreEqual(3, results.Count());
        }

        [TestMethod]
        public void CreateMany_WithSpecificCount_ShouldReturnSpecifiedNumberOfItems()
        {
            // Arrange
            var expectedCount = 5;

            // Act
            var results = _container.CreateMany<int>(expectedCount);

            // Assert
            Assert.AreEqual(expectedCount, results.Count());
        }

        [TestMethod]
        public void Build_ShouldReturnCustomizationComposer()
        {
            // Act
            var composer = _container.Build<TestModel>();

            // Assert
            Assert.IsNotNull(composer);
        }

        [TestMethod]
        public void Build_WithCustomization_ShouldRespectCustomization()
        {
            // Arrange
            var customName = "CustomName";

            // Act
            var result = _container.Build<TestModel>()
                .With(x => x.Name, customName)
                .Create();

            // Assert
            Assert.AreEqual(customName, result.Name);
        }

        [TestMethod]
        public void Resolve_ShouldReturnSameInstanceOnMultipleCalls()
        {
            // Act
            var first = _container.Resolve<TestModel>();
            var second = _container.Resolve<TestModel>();

            // Assert
            Assert.IsNotNull(first);
            Assert.IsNotNull(second);
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void ResolveMock_ShouldReturnMockInstance()
        {
            // Act
            var mock = _container.ResolveMock<ITestService>();

            // Assert
            Assert.IsNotNull(mock);
            Assert.IsNotNull(mock.Object);
        }

        [TestMethod]
        public void ResolveMock_ShouldReturnSameMockOnMultipleCalls()
        {
            // Act
            var first = _container.ResolveMock<ITestService>();
            var second = _container.ResolveMock<ITestService>();

            // Assert
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void Register_WithInstance_ShouldReturnRegisteredInstance()
        {
            // Arrange
            var instance = new TestModel { Name = "RegisteredInstance", Id = 123 };

            // Act
            _container.Register(instance);
            var result = _container.Resolve<TestModel>();

            // Assert
            Assert.AreSame(instance, result);
            Assert.AreEqual("RegisteredInstance", result.Name);
            Assert.AreEqual(123, result.Id);
        }

        [TestMethod]
        public void Register_WithInterfaceAndInstance_ShouldReturnRegisteredInstance()
        {
            // Arrange
            var instance = new TestService();

            // Act
            _container.Register<ITestService, TestService>(instance);
            var result = _container.Resolve<ITestService>();

            // Assert
            Assert.AreSame(instance, result);
        }

        [TestMethod]
        public void AddCustomizations_WithValidBuilders_ShouldAddToCustomizations()
        {
            // Arrange
            var builder = new Mock<ISpecimenBuilder>();

            // Act & Assert - Should not throw
            _container.AddCustomizations(builder.Object);
        }

        [TestMethod]
        public void AddCustomizations_WithNullBuilders_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _container.AddCustomizations(null));
        }

        [TestMethod]
        public void AddCustomizations_WithNullBuilderInArray_ShouldThrowArgumentNullException()
        {
            // Arrange
            var validBuilder = new Mock<ISpecimenBuilder>();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _container.AddCustomizations(validBuilder.Object, null));
        }

        public class TestModel
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public interface ITestService
        {
            void DoSomething();
        }

        public class TestService : ITestService
        {
            public void DoSomething()
            {
                // Implementation
            }
        }
    }
}