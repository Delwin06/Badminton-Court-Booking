using BadmintonCourtBookingSystem.Data;
using BadmintonCourtBookingSystem.Models;
using BadmintonCourtBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace BadmintonCourtBookingSystem.Controllers;

[Authorize]
public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ==========================
    // BOOKING LIST
    // ==========================
    public async Task<IActionResult> Index()
    {
        var bookings = await _context.Bookings
            .Include(b => b.Court)
            .Include(b => b.TimeSlot)
            .Include(b => b.User)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

        return View(bookings);
    }

    // ==========================
    // CREATE BOOKING (GET)
    // ==========================
    public async Task<IActionResult> Create()
    {
        var model = new BookingCreateViewModel();

        model.Courts = await _context.Courts
            .Select(c => new SelectListItem
            {
                Value = c.CourtId.ToString(),
                Text = c.CourtName
            })
            .ToListAsync();

        model.TimeSlots = await _context.TimeSlots
            .Where(t => t.IsActive)
            .OrderBy(t => t.StartTime)
            .Select(t => new SelectListItem
            {
                Value = t.TimeSlotId.ToString(),
                Text = t.StartTime + " - " + t.EndTime
            })
            .ToListAsync();

        model.BookingDate = DateOnly.FromDateTime(DateTime.Today);

        // ADD THIS BLOCK HERE
        ViewBag.CourtsData = await _context.Courts
            .Select(c => new
            {
                c.CourtId,
                c.CourtName,
                c.HourlyRate
            })
            .ToListAsync();

        return View(model);
    }

    // ==========================
    // CREATE BOOKING (POST)
    // ==========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
      BookingCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns(model);

            ViewBag.CourtsData = await _context.Courts
                .Select(c => new
                {
                    c.CourtId,
                    c.CourtName,
                    c.HourlyRate
                })
                .ToListAsync();

            return View(model);
        }

        bool exists = await _context.Bookings.AnyAsync(b =>
            b.CourtId == model.CourtId &&
            b.TimeSlotId == model.TimeSlotId &&
            b.BookingDate == model.BookingDate &&
            b.BookingStatus != "Cancelled");

        if (exists)
        {
            ModelState.AddModelError("",
                "This court is already booked for the selected slot.");

            await LoadDropdowns(model);

            ViewBag.CourtsData = await _context.Courts
                .Select(c => new
                {
                    c.CourtId,
                    c.CourtName,
                    c.HourlyRate
                })
                .ToListAsync();

            return View(model);
        }

        var court = await _context.Courts
            .FirstOrDefaultAsync(c => c.CourtId == model.CourtId);

        if (court == null)
            return NotFound();

        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        int userId = int.Parse(userIdClaim);

        var booking = new Booking
        {
            UserId = userId,
            CourtId = model.CourtId,
            TimeSlotId = model.TimeSlotId,
            BookingDate = model.BookingDate,
            Notes = model.Notes,
            Amount = court.HourlyRate,

            BookingStatus = "Pending",
            PaymentStatus = "Pending",
            CheckInStatus = false,

            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);

        await _context.SaveChangesAsync();

        await CreateNotification(
            userId,
            $"Booking #{booking.BookingReference} created successfully.");

        TempData["Success"] =
            "Booking created successfully.";

        return RedirectToAction(
            nameof(Success),
            new { id = booking.BookingId });
    }

    // ==========================
    // BOOKING DETAILS
    // ==========================
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var booking = await _context.Bookings
            .Include(b => b.Court)
            .Include(b => b.TimeSlot)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.BookingId == id);

        if (booking == null)
            return NotFound();

        return View(booking);
    }

    // ==========================
    // CONFIRM BOOKING
    // ==========================
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Confirm(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        if (booking.BookingStatus == "Cancelled")
        {
            TempData["Error"] =
                "Cancelled bookings cannot be confirmed.";

            return RedirectToAction(nameof(Details),
                new { id });
        }

        booking.BookingStatus = "Confirmed";
        booking.ConfirmedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await CreateNotification(
    booking.UserId,
    $"Booking #{booking.BookingReference} confirmed.");

        TempData["Success"] =
            "Booking confirmed successfully.";

        return RedirectToAction(nameof(Details),
            new { id });
    }

    // ==========================
    // CHECK IN
    // ==========================
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> CheckIn(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        if (booking.BookingStatus != "Confirmed")
        {
            TempData["Error"] =
                "Only confirmed bookings can be checked in.";

            return RedirectToAction(nameof(Details),
                new { id });
        }

        booking.CheckInStatus = true;
        booking.BookingStatus = "Checked In";
        booking.CheckedInAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await CreateNotification(
    booking.UserId,
    $"Checked in successfully.");

        TempData["Success"] =
            "Customer checked in successfully.";

        return RedirectToAction(nameof(Details),
            new { id });
    }

    // ==========================
    // COMPLETE BOOKING
    // ==========================
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Complete(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        if (!booking.CheckInStatus)
        {
            TempData["Error"] =
                "Booking must be checked in first.";

            return RedirectToAction(nameof(Details),
                new { id });
        }

        booking.BookingStatus = "Completed";
        booking.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await CreateNotification(
    booking.UserId,
    $"Booking completed successfully.");

        TempData["Success"] =
            "Booking completed successfully.";

        return RedirectToAction(nameof(Details),
            new { id });
    }

    // ==========================
    // CANCEL BOOKING
    // ==========================
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Cancel(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        booking.BookingStatus = "Cancelled";

        await _context.SaveChangesAsync();
        await CreateNotification(
    booking.UserId,
    $"Booking cancelled.");

        TempData["Success"] =
            "Booking cancelled successfully.";

        return RedirectToAction(nameof(Details),
            new { id });
    }

    // ==========================
    // MY BOOKINGS
    // ==========================
    [Authorize]
    public async Task<IActionResult> MyBookings(string status = "")
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return RedirectToAction("Login", "Account");
        }

        int userId = int.Parse(userIdClaim);

        var bookings = await _context.Bookings
            .Include(b => b.Court)
            .Include(b => b.TimeSlot)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

        if (!string.IsNullOrEmpty(status))
        {
            bookings = bookings
                .Where(b => b.BookingStatus == status)
                .ToList();
        }

        ViewBag.SelectedStatus = status;

        return View(bookings);
    }
    // ==========================
    // LOAD DROPDOWNS
    // ==========================
    private async Task LoadDropdowns(
        BookingCreateViewModel model)
    {
        model.Courts = await _context.Courts
            .Select(c => new SelectListItem
            {
                Value = c.CourtId.ToString(),
                Text = c.CourtName
            })
            .ToListAsync();

        model.TimeSlots = await _context.TimeSlots
            .Where(t => t.IsActive)
            .OrderBy(t => t.StartTime)
            .Select(t => new SelectListItem
            {
                Value = t.TimeSlotId.ToString(),
                Text = t.StartTime + " - " + t.EndTime
            })
            .ToListAsync();
    }

    private async Task CreateNotification(
    int userId,
    string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();
    }

    public async Task<IActionResult> Success(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Court)
            .Include(b => b.TimeSlot)
            .FirstOrDefaultAsync(b => b.BookingId == id);

        if (booking == null)
            return NotFound();

        return View(booking);
    }
}