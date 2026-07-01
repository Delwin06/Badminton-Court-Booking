namespace BadmintonCourtBookingSystem.Models;

public class Review
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int CourtId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }

    public Court? Court { get; set; }
}