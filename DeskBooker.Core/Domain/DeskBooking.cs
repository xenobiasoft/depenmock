namespace DeskBooker.Core.Domain;

public record DeskBooking : DeskBookingBase
{
	public int Id { get; set; }
	public int DeskId { get; set; }
}