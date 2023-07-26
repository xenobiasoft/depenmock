using DepenMock.Loggers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DepenMock.NUnit
{
	public abstract class BaseTest
	{

		[SetUp]
		public void BaseSetup()
		{
			Container = new Container();
			Logger = new ListLogger();
			Container
				.ResolveMock<ILoggerFactory>()
				.Setup(x => x.CreateLogger(It.IsAny<string>()))
				.Returns(Logger);
		}

		public Container Container { get; private set; }

		protected virtual ListLogger Logger { get; private set; }
	}
}
