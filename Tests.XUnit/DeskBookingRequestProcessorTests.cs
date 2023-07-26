﻿using DeskBooker.Core.Domain;
using DeskBooker.Core.Interfaces;
using DeskBooker.Core.Processor;
using Moq;
using Xunit;
using AutoFixture;
using DepenMock.XUnit;

namespace Tests.XUnit;

public class DeskBookingRequestProcessorTests : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
	[Fact]
	public void BookDesk_WhenDeskAvailable_ReturnsBookedDeskResult()
	{
		// Assemble
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
		var actualResult = sut.BookDesk(request);

		// Assert
		Assert.Equal(expectedResult, actualResult);
	}

	[Fact]
	public void BookDesk_WhenDeskIsNull_ThrowsException()
	{
		// Assemble
		var sut = ResolveSut();

		// Act
		void BookDesk() => sut.BookDesk(null);

		// Assert
		Assert.Throws<ArgumentNullException>(BookDesk);
	}

	[Fact]
	public void BookDesk_WhenDeskAvailable_BooksDesk()
	{
		// Assemble
		var mockRepo = Container.ResolveMock<IDeskBookingRepository>();
		Container
			.ResolveMock<IDeskRepository>()
			.Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
			.Returns(Container.CreateMany<Desk>());

		var sut = ResolveSut();

		// Act
		sut.BookDesk(Container.Create<DeskBookingRequest>());

		// Assert
		mockRepo.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);
	}

	[Fact]
	public void BookDesk_WhenNoDeskAvailable_DoesNotBookDesk()
	{
		// Assemble
		Container
			.ResolveMock<IDeskRepository>()
			.Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
			.Returns(new List<Desk>());
		var mockRepo = Container.ResolveMock<IDeskBookingRepository>();
		var sut = ResolveSut();

		// Act
		sut.BookDesk(Container.Create<DeskBookingRequest>());

		// Assert
		mockRepo.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
	}

	[Fact]
	public void BookDesk_WhenNoAvailableDesks_ReturnStatusNoDeskAvailable()
	{
		// Assemble
		Container
			.ResolveMock<IDeskRepository>()
			.Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
			.Returns(new List<Desk>());

		var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>());

		// Assert
		Assert.Equal(DeskBookingResultCode.NoDeskAvailable, result.Code);
	}

	[Fact]
	public void BookDesk_WhenDeskAvailable_ReturnStatusAvailableDesks()
	{
		// Assemble
		var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>());

		// Assert
		Assert.Equal(DeskBookingResultCode.Success, result.Code);
	}

	[Fact]
	public void BookDesk_WhenNoDeskAvailable_ReturnsEmptyDeskBookingId()
	{
		// Assemble
		Container
			.ResolveMock<IDeskRepository>()
			.Setup(x => x.GetAvailableDesks(It.IsAny<DateTime>()))
			.Returns(new List<Desk>());

		var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>());

		// Assert
		Assert.Null(result.DeskBookingId);
	}

	[Fact]
	public void BookDesk_WhenDeskAvailable_ReturnsDeskBookingId()
	{
		// Assemble
		var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>());

		// Assert
		Assert.Equal(0, result.DeskBookingId);
	}
}