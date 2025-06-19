using AutoFixture;
using DepenMock.NUnit;
using DeskBooker.Core.Domain;
using DeskBooker.Core.Interfaces;
using DeskBooker.Core.Processor;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using DepenMock;

namespace Tests.NUnit;

[TestFixture]
public class DeskBookingRequestProcessorTests : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
	[Test]
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
		Assert.That(expectedResult, Is.EqualTo(actualResult));
	}

	[Test]
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

    [Test]
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

    [Test]
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

	[Test]
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

	[Test]
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
		Assert.That(DeskBookingResultCode.NoDeskAvailable, Is.EqualTo(result.Code));
	}

	[Test]
	public void BookDesk_WhenDeskAvailables_ReturnStatusAvailableDesks()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

		// Assert
		Assert.That(DeskBookingResultCode.Success, Is.EqualTo(result.Code));
	}

	[Test]
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
		Assert.That(null, Is.EqualTo(result.DeskBookingId));
	}

	[Test]
	public void BookDesk_WhenDeskAvailable_ReturnsDeskBookingId()
	{
        // Assemble
        var correlationId = Container.Create<string>();
        var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

		// Assert
		Assert.That(0, Is.EqualTo(result.DeskBookingId));
	}
}