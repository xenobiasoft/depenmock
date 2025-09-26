using DeskBooker.Core.Domain;
using DeskBooker.Core.Interfaces;
using DeskBooker.Core.Processor;
using Moq;
using Xunit;
using AutoFixture;
using DepenMock;
using DepenMock.XUnit;

namespace Tests.XUnit;

public class DeskBookingRequestProcessorTests : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
	[Fact]
	public void BookDesk_WhenDeskAvailable_ReturnsBookedDeskResult()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        var request = Container.Create<DeskBookingRequest>();
		var expectedResult = Container
			.Build<DeskBookingResult>()
			.With(x => x.DeskBookingId, 0)
			.With(x => x.Code, DeskBookingResultCode.Success)
			.With(x => x.FirstName, request.FirstName)
			.With(x => x.LastName, request.LastName)
			.With(x => x.Email, request.Email)
			.With(x => x.Date, request.Date)
			.Create();
		var sut = ResolveSut();

		// Act
		var actualResult = sut.BookDesk(request, correlationId);

		// Assert
		Assert.Equal(expectedResult, actualResult);
    }

    [Fact]
	public void BookDesk_WhenDeskIsNull_ThrowsException()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        var sut = ResolveSut();

		// Act
		void BookDesk() => sut.BookDesk(null, correlationId);

		// Assert
		Assert.Throws<ArgumentNullException>(BookDesk);
	}

    [Fact]
    public void BookDesk_WhenDeskIsNull_LogsError()
    {
        // Assemble
        var correlationId = Container.Create<string>();
        var sut = ResolveSut();

        try
        {
            // Act
            sut.BookDesk(null, correlationId);
        }
        catch
        {
            // Assert
            Logger.ErrorLogs().ContainsMessage($"Correlation Id: {correlationId}");
        }
    }

    [Fact]
	public void BookDesk_WhenDeskAvailable_BooksDesk()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        var mockRepo = Container.ResolveMock<IDeskBookingRepository>();
		Container
			.ResolveMock<IDeskRepository>()
			.Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
			.Returns(Container.CreateMany<Desk>());

		var sut = ResolveSut();

		// Act
		sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

		// Assert
		mockRepo.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);
	}

	[Fact]
	public void BookDesk_WhenNoDeskAvailable_DoesNotBookDesk()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        Container
			.ResolveMock<IDeskRepository>()
			.Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
			.Returns(new List<Desk>());
		var mockRepo = Container.ResolveMock<IDeskBookingRepository>();
		var sut = ResolveSut();

		// Act
		sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

		// Assert
		mockRepo.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
	}

	[Fact]
	public void BookDesk_WhenNoAvailableDesks_ReturnStatusNoDeskAvailable()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        Container
			.ResolveMock<IDeskRepository>()
			.Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
			.Returns(new List<Desk>());

		var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

		// Assert
		Assert.Equal(DeskBookingResultCode.NoDeskAvailable, result.Code);
	}

	[Fact]
	public void BookDesk_WhenDeskAvailable_ReturnStatusAvailableDesks()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

		// Assert
		Assert.Equal(DeskBookingResultCode.Success, result.Code);
	}

	[Fact]
	public void BookDesk_WhenNoDeskAvailable_ReturnsEmptyDeskBookingId()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        Container
			.ResolveMock<IDeskRepository>()
			.Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
			.Returns(new List<Desk>());

		var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

		// Assert
		Assert.Null(result.DeskBookingId);
	}

	[Fact]
	public void BookDesk_WhenDeskAvailable_ReturnsDeskBookingId()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

		// Assert
		Assert.Equal(0, result.DeskBookingId);
	}

    [Fact]
    public void ErrorLogs_WhenNoLogsExist_ShouldReturnEmptyList()
    {
        // Arrange & Act & Assert - This should not throw an exception
        var errorLogs = Logger.ErrorLogs();
        
        // Assert
        Assert.Empty(errorLogs);
    }

    [Fact]
    public void InformationLogs_WhenNoLogsExist_ShouldReturnEmptyList()
    {
        // Arrange & Act & Assert - This should not throw an exception
        var infoLogs = Logger.InformationLogs();
        
        // Assert
        Assert.Empty(infoLogs);
    }

    [Fact]
    public void WarningLogs_WhenNoLogsExist_ShouldReturnEmptyList()
    {
        // Arrange & Act & Assert - This should not throw an exception
        var warningLogs = Logger.WarningLogs();
        
        // Assert
        Assert.Empty(warningLogs);
    }

    [Fact]
    public void LogExtensions_ShouldWorkWithActualLogMessages()
    {
        // Arrange & Act - Create SUT and trigger some logging
        var correlationId = Container.Create<string>();
        var sut = ResolveSut();
        
        try
        {
            sut.BookDesk(null, correlationId); // This will log an error
        }
        catch
        {
            // Expected exception, we're testing the logging
        }

        // Assert - Now we should have error logs
        var errorLogs = Logger.ErrorLogs();
        Assert.NotEmpty(errorLogs);
        
        // And we should still get empty lists for log types that haven't been used
        var warningLogs = Logger.WarningLogs();
        var infoLogs = Logger.InformationLogs();
        Assert.Empty(warningLogs);
        Assert.Empty(infoLogs);
    }
}