using BadmintonCourtBookingSystem.Data;
using BadmintonCourtBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BadmintonCourtBookingSystem.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AnalyticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AnalyticsViewModel();

            //-----------------------------------------
            // Dashboard Summary Cards
            //-----------------------------------------

            model.TotalBookings = await _context.Bookings.CountAsync();

            model.TotalRevenue = await _context.Bookings
                .Where(b => b.BookingStatus != "Cancelled")
                .SumAsync(b => (decimal?)b.Amount) ?? 0;

            model.PendingBookings = await _context.Bookings
                .CountAsync(b => b.BookingStatus == "Pending");

            model.ConfirmedBookings = await _context.Bookings
                .CountAsync(b => b.BookingStatus == "Confirmed");

            model.CancelledBookings = await _context.Bookings
                .CountAsync(b => b.BookingStatus == "Cancelled");

            //-----------------------------------------
            // Bookings Per Day Chart (Last 7 Days)
            //-----------------------------------------

            var bookingPerDay = await _context.Bookings
                .Where(b => b.CreatedAt >= DateTime.Today.AddDays(-6))
                .GroupBy(b => b.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            model.BookingDates = bookingPerDay
                .Select(x => x.Date.ToString("dd MMM"))
                .ToList();

            model.BookingCounts = bookingPerDay
                .Select(x => x.Count)
                .ToList();

            //-----------------------------------------
            // Revenue Per Day Chart (Last 7 Days)
            //-----------------------------------------

            var revenuePerDay = await _context.Bookings
                .Where(b =>
                    b.BookingStatus != "Cancelled" &&
                    b.CreatedAt >= DateTime.Today.AddDays(-6))
                .GroupBy(b => b.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            model.RevenueDates = revenuePerDay
                .Select(x => x.Date.ToString("dd MMM"))
                .ToList();

            model.RevenueAmounts = revenuePerDay
                .Select(x => x.Revenue)
                .ToList();

            //-----------------------------------------
            // Booking Status Distribution
            //-----------------------------------------

            var statusData = await _context.Bookings
                .GroupBy(b => b.BookingStatus)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            model.StatusLabels = statusData
                .Select(x => x.Status)
                .ToList();

            model.StatusCounts = statusData
                .Select(x => x.Count)
                .ToList();

            //-----------------------------------------
            // Monthly Revenue Chart
            //-----------------------------------------

            var monthlyRevenue = await _context.Bookings
                .Where(b => b.BookingStatus != "Cancelled")
                .GroupBy(b => new
                {
                    b.CreatedAt.Year,
                    b.CreatedAt.Month
                })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            model.MonthlyLabels = monthlyRevenue
                .Select(x =>
                    new DateTime(x.Year, x.Month, 1)
                    .ToString("MMM yyyy"))
                .ToList();

            model.MonthlyRevenue = monthlyRevenue
                .Select(x => x.Revenue)
                .ToList();

            return View(model);
        }
    }
}