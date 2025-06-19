using DeskBooker.Core.Domain;
using System;
using System.Linq;
using DeskBooker.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace DeskBooker.Core.Processor;

public class DeskBookingRequestProcessor(
    IDeskBookingRepository deskBookingRepository,
    IDeskRepository deskRepository,
    ILogger<DeskBookingRequestProcessor> logger)
    : IDeskBookingRequestProcessor
{
    public DeskBookingResult BookDesk(DeskBookingRequest request, string correlationId)
	{
		if (request == null)
		{
			logger.LogError("Desk booking request is null. Correlation Id: {CorrelationId}", correlationId);
            throw new ArgumentNullException(nameof(request));
		}

		var result = request.Create<DeskBookingResult>();

		var availableDesks = deskRepository.GetAvailableDesks(request.Date);

		if (availableDesks.Any())
		{
			var availableDesk = availableDesks.First();
			var deskBooking = request.Create<DeskBooking>();
			deskBooking.DeskId = availableDesk.Id;

			deskBookingRepository.Save(deskBooking);

			result.DeskBookingId = deskBooking.Id;
			result.Code = DeskBookingResultCode.Success;
		}
		else
		{
			result.Code = DeskBookingResultCode.NoDeskAvailable;
		}

		logger.LogInformation("Desk booking result: {ResultCode} for request on {RequestDate}.", result.Code, request.Date);

        return result;
	}
}
