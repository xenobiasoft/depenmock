﻿using AutoFixture;
using DepenMock.NUnit;
using DeskBooker.Core.Domain;
using DeskBooker.Core.Interfaces;
using DeskBooker.Core.Processor;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests.NUnit;

[TestFixture]
public class DeskBookingRequestProcessorTests : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
	[Test]
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
		Assert.AreEqual(expectedResult, actualResult);
	}

	[Test]
	public void BookDesk_WhenDeskIsNull_ThrowsException()
	{
		// Assemble
		var sut = ResolveSut();

		// Act
		void BookDesk() => sut.BookDesk(null);

		// Assert
		Assert.Throws<ArgumentNullException>(BookDesk);
	}

	[Test]
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

	[Test]
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

	[Test]
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
		Assert.AreEqual(DeskBookingResultCode.NoDeskAvailable, result.Code);
	}

	[Test]
	public void BookDesk_WhenDeskAvailables_ReturnStatusAvailableDesks()
	{
		// Assemble
		var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>());

		// Assert
		Assert.AreEqual(DeskBookingResultCode.Success, result.Code);
	}

	[Test]
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
		Assert.AreEqual(null, result.DeskBookingId);
	}

	[Test]
	public void BookDesk_WhenDeskAvailable_ReturnsDeskBookingId()
	{
		// Assemble
		var sut = ResolveSut();

		// Act
		var result = sut.BookDesk(Container.Create<DeskBookingRequest>());

		// Assert
		Assert.AreEqual(0, result.DeskBookingId);
	}
}