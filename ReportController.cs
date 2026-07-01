using BadmintonCourtBookingSystem.Data;
using BadmintonCourtBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BadmintonCourtBookingSystem.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class ReportController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(DateOnly? reportDate)
    {
        DateOnly selectedDate =
            reportDate ?? DateOnly.FromDateTime(DateTime.Today);

        var bookings = await _context.Bookings
            .Where(b => b.BookingDate == selectedDate)
            .ToListAsync();

        BookingReportViewModel model =
            new BookingReportViewModel
            {
                ReportDate = selectedDate,

                TotalBookings = bookings.Count,

                PendingBookings = bookings.Count(b =>
                    b.BookingStatus == "Pending"),

                ConfirmedBookings = bookings.Count(b =>
                    b.BookingStatus == "Confirmed"),

                CheckedInBookings = bookings.Count(b =>
                    b.CheckInStatus),

                CompletedBookings = bookings.Count(b =>
                    b.BookingStatus == "Completed"),

                CancelledBookings = bookings.Count(b =>
                    b.BookingStatus == "Cancelled"),

                TotalRevenue = bookings
                    .Where(b => b.BookingStatus == "Confirmed")
                    .Sum(b => b.Amount)
            };

        return View(model);
    }
}