namespace BadmintonCourtBookingSystem.Models;

public class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}