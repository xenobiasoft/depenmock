using DeskBooker.Core.Domain;

namespace DeskBooker.Core;

public static class ExtensionMethods
{
	public static T Create<T>(this DeskBookingRequest request) where T : DeskBookingBase, new()
	{
		return new T
		{
			FirstName = request.FirstName,
			LastName = request.LastName,
			Email = request.Email,
			Date = request.Date
		};
	}
}