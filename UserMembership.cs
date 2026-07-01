namespace BadmintonCourtBookingSystem.Models;

public class UserMembership
{
    public int UserMembershipId { get; set; }

    public int UserId { get; set; }

    public int MembershipPlanId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public User? User { get; set; }

    public MembershipPlan? MembershipPlan { get; set; }
}