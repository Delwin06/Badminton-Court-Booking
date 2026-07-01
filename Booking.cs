using BadmintonCourtBookingSystem.Models;

namespace BadmintonCourtBookingSystem.Models;

public class Booking
{
    public int BookingId { get; set; }

    public Guid BookingReference { get; set; } = Guid.NewGuid();

    public int UserId { get; set; }

    public int CourtId { get; set; }

    public int TimeSlotId { get; set; }

    public DateOnly BookingDate { get; set; }

    public decimal Amount { get; set; }

    public string BookingStatus { get; set; } = "Pending";

    public string PaymentStatus { get; set; } = "Pending";

    public bool CheckInStatus { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CheckedInAt { get; set; }

    public User? User { get; set; }

    public Court? Court { get; set; }

    public TimeSlot? TimeSlot { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    
    public DateTime? CompletedAt { get; set; }

   



}