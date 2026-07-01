using System.ComponentModel.DataAnnotations;

namespace BadmintonCourtBookingSystem.Models;

public class CourtImage
{
    [Key]
    public int ImageId { get; set; }

    public int CourtId { get; set; }

    public string ImagePath { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public Court? Court { get; set; }
}