using AutoFixture;
using DepenMock;
using DepenMock.Attributes;
using DepenMock.FakeItEasy;
using DeskBooker.Core.Domain;
using DeskBooker.Core.Interfaces;
using DeskBooker.Core.Processor;
using FakeItEasy;

namespace Tests.FakeItEasy;


[LogOutput(LogOutputTiming.Always)]
public class DeskBookingRequestProcessorTests : FakeItEasyBaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
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
            Logger.ErrorLogs().AssertContains($"Correlation Id: {correlationId}");
        }
    }

    [Fact]
	public void BookDesk_WhenDeskAvailable_BooksDesk()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        var fakeBookingRepo = Container.ResolveMock<IDeskBookingRepository>().AsFake();
        var fakeDeskRepo = Container.ResolveMock<IDeskRepository>().AsFake();
        A.CallTo(() => fakeDeskRepo.GetAvailableDesks(A<DateTime>._))
            .Returns(Container.CreateMany<Desk>());

		var sut = ResolveSut();

		// Act
		sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

        // Assert
        A.CallTo(() => fakeBookingRepo.Save(A<DeskBooking>._)).MustHaveHappenedOnceExactly();
    }

	[Fact]
	public void BookDesk_WhenNoDeskAvailable_DoesNotBookDesk()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        var fakeDeskRepo = Container.ResolveMock<IDeskRepository>().AsFake();
        A.CallTo(() => fakeDeskRepo.GetAvailableDesks(A<DateTime>._))
            .Returns(new List<Desk>());
        var fakeBookingRepo = Container.ResolveMock<IDeskBookingRepository>().AsFake();
		var sut = ResolveSut();

		// Act
		sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

		// Assert
		A.CallTo(() => fakeBookingRepo.Save(A<DeskBooking>._)).MustNotHaveHappened();
	}

	[Fact]
	public void BookDesk_WhenNoAvailableDesks_ReturnStatusNoDeskAvailable()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        Container.SetupMock<IDeskRepository>(f => A
            .CallTo(() => f.GetAvailableDesks(A<DateTime>._))
            .Returns(new List<Desk>()));

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
        var fakeDeskRepo = Container.ResolveMock<IDeskRepository>().AsFake();
        A.CallTo(() => fakeDeskRepo.GetAvailableDesks(A<DateTime>._))
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
}
