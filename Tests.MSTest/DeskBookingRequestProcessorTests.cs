using AutoFixture;
using DepenMock;
using DepenMock.MSTest;
using DeskBooker.Core.Domain;
using DeskBooker.Core.Interfaces;
using DeskBooker.Core.Processor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests.MSTest;

[TestClass]
public class DeskBookingRequestProcessorTests : BaseTestByAbstraction<DeskBookingRequestProcessor, IDeskBookingRequestProcessor>
{
    [TestMethod]
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
        Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BookDesk_WhenDeskIsNull_ThrowsException()
    {
        // Assemble
        var correlationId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        sut.BookDesk(null, correlationId);
    }

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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
        Assert.AreEqual(DeskBookingResultCode.NoDeskAvailable, result.Code);
    }

    [TestMethod]
    public void BookDesk_WhenDeskAvailable_ReturnStatusAvailableDesks()
    {
        // Assemble
        var correlationId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        var result = sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

        // Assert
        Assert.AreEqual(DeskBookingResultCode.Success, result.Code);
    }

    [TestMethod]
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
        Assert.IsNull(result.DeskBookingId);
    }

    [TestMethod]
    public void BookDesk_WhenDeskAvailable_ReturnsDeskBookingId()
    {
        // Assemble
        var correlationId = Container.Create<string>();
        var sut = ResolveSut();

        // Act
        var result = sut.BookDesk(Container.Create<DeskBookingRequest>(), correlationId);

        // Assert
        Assert.AreEqual(0, result.DeskBookingId);
    }
}