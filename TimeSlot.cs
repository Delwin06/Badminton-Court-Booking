namespace BadmintonCourtBookingSystem.Models;

public class TimeSlot
{
    public int TimeSlotId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Booking> Bookings { get; set; }
        = new List<Booking>();
}