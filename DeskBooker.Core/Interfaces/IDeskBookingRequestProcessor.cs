using DeskBooker.Core.Domain;

namespace DeskBooker.Core.Interfaces;

public interface IDeskBookingRequestProcessor
{
	DeskBookingResult BookDesk(DeskBookingRequest request);
}
