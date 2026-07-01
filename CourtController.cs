using BadmintonCourtBookingSystem.Data;
using BadmintonCourtBookingSystem.Models;
using BadmintonCourtBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BadmintonCourtBookingSystem.Controllers;

[Authorize]
public class CourtController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public CourtController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // =========================
    // COURT LIST (PUBLIC)
    // =========================

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var courts = await _context.Courts
            .Include(c => c.CourtImages)
            .Include(c => c.Reviews)
            .ToListAsync();

        return View(courts);
    }

    // =========================
    // COURT DETAILS (PUBLIC)
    // =========================

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var court = await _context.Courts
            .Include(c => c.CourtImages)
            .FirstOrDefaultAsync(c => c.CourtId == id);

        if (court == null)
        {
            return NotFound();
        }

        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.CourtId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var model = new CourtDetailsViewModel
        {
            Court = court,
            Reviews = reviews,
            TotalReviews = reviews.Count,
            AverageRating = reviews.Any()
                ? reviews.Average(r => r.Rating)
                : 0
        };

        return View(model);
    }

    // =========================
    // CREATE COURT (ADMIN/STAFF)
    // =========================

    [Authorize(Roles = "Admin,Staff")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create(Court court)
    {
        if (!ModelState.IsValid)
        {
            return View(court);
        }

        _context.Courts.Add(court);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Court created successfully.";

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // EDIT COURT (ADMIN/STAFF)
    // =========================

    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Edit(int id)
    {
        var court = await _context.Courts.FindAsync(id);

        if (court == null)
        {
            return NotFound();
        }

        return View(court);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Edit(Court court)
    {
        if (!ModelState.IsValid)
        {
            return View(court);
        }

        _context.Courts.Update(court);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Court updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // DELETE COURT (ADMIN/STAFF)
    // =========================

    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Delete(int id)
    {
        var court = await _context.Courts.FindAsync(id);

        if (court == null)
        {
            return NotFound();
        }

        return View(court);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var court = await _context.Courts.FindAsync(id);

        if (court != null)
        {
            _context.Courts.Remove(court);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Court deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // UPLOAD IMAGE (ADMIN/STAFF)
    // =========================

    [Authorize(Roles = "Admin,Staff")]
    public IActionResult UploadImage(int courtId)
    {
        var model = new CourtImageUploadViewModel
        {
            CourtId = courtId
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UploadImage(
        CourtImageUploadViewModel model)
    {
        if (model.ImageFile == null)
        {
            ModelState.AddModelError("", "Please select an image.");
            return View(model);
        }

        string uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "courts");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string fileName =
            Guid.NewGuid().ToString() +
            Path.GetExtension(model.ImageFile.FileName);

        string filePath =
            Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(
            filePath,
            FileMode.Create))
        {
            await model.ImageFile.CopyToAsync(stream);
        }

        var courtImage = new CourtImage
        {
            CourtId = model.CourtId,
            ImagePath = "/uploads/courts/" + fileName,
            DisplayOrder = 1
        };

        _context.CourtImages.Add(courtImage);

        await _context.SaveChangesAsync();

        TempData["Success"] = "Image uploaded successfully.";

        return RedirectToAction(
            nameof(Details),
            new { id = model.CourtId });
    }
}