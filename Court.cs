namespace BadmintonCourtBookingSystem.Models;

public class Court
{
    public int CourtId { get; set; }

    public string CourtName { get; set; } = string.Empty;

    public decimal HourlyRate { get; set; }

    public string CourtStatus { get; set; } = "Available";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CourtImage> CourtImages { get; set; } = new List<CourtImage>();

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}