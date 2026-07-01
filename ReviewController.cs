using BadmintonCourtBookingSystem.Data;
using BadmintonCourtBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BadmintonCourtBookingSystem.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReviewController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET
    public async Task<IActionResult> Create(int courtId)
    {
        var court = await _context.Courts
            .FirstOrDefaultAsync(c => c.CourtId == courtId);

        if (court == null)
        {
            return NotFound();
        }

        ViewBag.CourtName = court.CourtName;
        ViewBag.CourtId = courtId;

        return View();
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int courtId,
        int rating,
        string? comment)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var review = new Review
        {
            UserId = userId,
            CourtId = courtId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Review submitted successfully.";

        return RedirectToAction(
            "MyBookings",
            "Booking");
    }

    // ADMIN VIEW
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Index()
    {
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Court)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(reviews);
    }
}