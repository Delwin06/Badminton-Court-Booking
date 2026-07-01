namespace BadmintonCourtBookingSystem.Models;

public class MembershipPlan
{
    public int MembershipPlanId { get; set; }

    public string PlanName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int DurationDays { get; set; }

    public decimal DiscountPercentage { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
}