namespace DeskBooker.Core.Domain;

public record DeskBookingResult : DeskBookingBase
{
	public DeskBookingResultCode Code { get; set; }
	public int? DeskBookingId { get; set; }
}
