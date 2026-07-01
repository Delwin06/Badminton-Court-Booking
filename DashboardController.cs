using BadmintonCourtBookingSystem.Data;
using BadmintonCourtBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BadmintonCourtBookingSystem.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        DashboardViewModel model = new DashboardViewModel();

        model.TotalBookings =
            await _context.Bookings.CountAsync();

        model.ConfirmedBookings =
            await _context.Bookings
                .CountAsync(b => b.BookingStatus == "Confirmed");

        model.PendingBookings =
            await _context.Bookings
                .CountAsync(b => b.BookingStatus == "Pending");

        model.CancelledBookings =
            await _context.Bookings
                .CountAsync(b => b.BookingStatus == "Cancelled");

        model.TotalRevenue =
            await _context.Bookings
                .Where(b => b.BookingStatus == "Confirmed")
                .SumAsync(b => (decimal?)b.Amount) ?? 0;

        model.RecentBookings =
            await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Court)
                .OrderByDescending(b => b.CreatedAt)
                .Take(10)
                .ToListAsync();


        var today = DateOnly.FromDateTime(DateTime.Today);

        model.TodayBookings =
            await _context.Bookings
                .CountAsync(b => b.BookingDate == today);

        model.TodayRevenue =
            await _context.Bookings
                .Where(b => b.BookingDate == today &&
                            b.BookingStatus == "Confirmed")
                .SumAsync(b => (decimal?)b.Amount) ?? 0;

        model.ActiveBookings =
    await _context.Bookings
        .CountAsync(b =>
            b.BookingStatus == "Confirmed");

        model.TodayBookings =
    await _context.Bookings
        .CountAsync(b =>
            b.BookingDate == DateOnly.FromDateTime(DateTime.Today));

        model.TodayRevenue =
    await _context.Bookings
        .Where(b =>
            b.BookingDate == DateOnly.FromDateTime(DateTime.Today)
            && b.BookingStatus == "Confirmed")
        .SumAsync(b => (decimal?)b.Amount) ?? 0;

        model.CheckedInBookings =
    await _context.Bookings
        .CountAsync(b => b.CheckInStatus);

        model.PendingBookings =
    await _context.Bookings
        .CountAsync(b => b.BookingStatus == "Pending");

        model.ConfirmedBookings =
            await _context.Bookings
                .CountAsync(b => b.BookingStatus == "Confirmed");

        model.CheckedInBookings =
            await _context.Bookings
                .CountAsync(b => b.CheckInStatus);

        model.CompletedBookings =
    await _context.Bookings
        .CountAsync(b => b.BookingStatus == "Completed");
        return View(model);
    }

    public async Task<IActionResult> Charts()
    {
        var currentYear = DateTime.Now.Year;

        var vm = new DashboardChartViewModel
        {
            PendingBookings = await _context.Bookings
                .CountAsync(x => x.BookingStatus == "Pending"),

            ConfirmedBookings = await _context.Bookings
                .CountAsync(x => x.BookingStatus == "Confirmed"),

            CheckedInBookings = await _context.Bookings
                .CountAsync(x => x.BookingStatus == "Checked In"),

            CompletedBookings = await _context.Bookings
                .CountAsync(x => x.BookingStatus == "Completed"),

            CancelledBookings = await _context.Bookings
                .CountAsync(x => x.BookingStatus == "Cancelled")
        };

        for (int month = 1; month <= 12; month++)
        {
            var count = await _context.Bookings
                .CountAsync(b =>
                    b.BookingDate.Year == currentYear &&
                    b.BookingDate.Month == month);

            vm.MonthlyBookings.Add(count);
        }

        return View(vm);
    }
}