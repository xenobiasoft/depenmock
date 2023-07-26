using DeskBooker.Core.Domain;
using System;
using System.Linq;
using DeskBooker.Core.Interfaces;

namespace DeskBooker.Core.Processor;

public class DeskBookingRequestProcessor : IDeskBookingRequestProcessor
{
	private readonly IDeskBookingRepository _deskBookingRepository;
	private readonly IDeskRepository _deskRepository;

	public DeskBookingRequestProcessor(IDeskBookingRepository deskBookingRepository, IDeskRepository deskRepository)
	{
		_deskBookingRepository = deskBookingRepository;
		_deskRepository = deskRepository;
	}

	public DeskBookingResult BookDesk(DeskBookingRequest request)
	{
		if (request == null)
		{
			throw new ArgumentNullException(nameof(request));
		}

		var result = request.Create<DeskBookingResult>();

		var availableDesks = _deskRepository.GetAvailableDesks(request.Date);

		if (availableDesks.Any())
		{
			var availableDesk = availableDesks.First();
			var deskBooking = request.Create<DeskBooking>();
			deskBooking.DeskId = availableDesk.Id;

			_deskBookingRepository.Save(deskBooking);

			result.DeskBookingId = deskBooking.Id;
			result.Code = DeskBookingResultCode.Success;
		}
		else
		{
			result.Code = DeskBookingResultCode.NoDeskAvailable;
		}

		return result;
	}
}
