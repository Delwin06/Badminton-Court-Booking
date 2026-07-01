using BadmintonCourtBookingSystem.Data;
using BadmintonCourtBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BadmintonCourtBookingSystem.Controllers;

[Authorize(Roles = "Admin,Staff")]
public class TimeSlotController : Controller
{
    private readonly ApplicationDbContext _context;

    public TimeSlotController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var timeSlots = await _context.TimeSlots
            .OrderBy(t => t.StartTime)
            .ToListAsync();

        return View(timeSlots);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TimeSlot timeSlot)
    {
        if (timeSlot.StartTime.Minute != 0 ||
    timeSlot.EndTime.Minute != 0)
        {
            ModelState.AddModelError("",
                "Time slots must be on full hours.");

            return View(timeSlot);
        }
        if (!ModelState.IsValid)
        {
            return View(timeSlot);
        }
        if ((timeSlot.EndTime.ToTimeSpan() -
     timeSlot.StartTime.ToTimeSpan()) != TimeSpan.FromHours(1))
        {
            ModelState.AddModelError("",
                "Each time slot must be exactly 1 hour.");

            return View(timeSlot);
        }

        if (timeSlot.EndTime <= timeSlot.StartTime)
        {
            ModelState.AddModelError("", "End Time must be greater than Start Time.");
            return View(timeSlot);
        }

        bool exists = await _context.TimeSlots.AnyAsync(t =>
            t.StartTime == timeSlot.StartTime &&
            t.EndTime == timeSlot.EndTime);

        if (exists)
        {
            ModelState.AddModelError("", "This time slot already exists.");
            return View(timeSlot);
        }

        _context.TimeSlots.Add(timeSlot);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var timeSlot = await _context.TimeSlots.FindAsync(id);

        if (timeSlot == null)
        {
            return NotFound();
        }

        return View(timeSlot);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TimeSlot timeSlot)
    {
        if (timeSlot.StartTime.Minute != 0 ||
    timeSlot.EndTime.Minute != 0)
        {
            ModelState.AddModelError("",
                "Time slots must be on full hours.");

            return View(timeSlot);
        }
        if (timeSlot.EndTime <= timeSlot.StartTime)
        {
            ModelState.AddModelError("",
                "End Time must be greater than Start Time.");

            return View(timeSlot);
        }
        if ((timeSlot.EndTime.ToTimeSpan() -
     timeSlot.StartTime.ToTimeSpan()) != TimeSpan.FromHours(1))
        {
            ModelState.AddModelError("",
                "Each time slot must be exactly 1 hour.");

            return View(timeSlot);
        }
        bool exists = await _context.TimeSlots.AnyAsync(t =>
    t.TimeSlotId != timeSlot.TimeSlotId &&
    t.StartTime == timeSlot.StartTime &&
    t.EndTime == timeSlot.EndTime);

        if (exists)
        {
            ModelState.AddModelError("",
                "This time slot already exists.");

            return View(timeSlot);
        }
        if (id != timeSlot.TimeSlotId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(timeSlot);
        }

        _context.Update(timeSlot);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var timeSlot = await _context.TimeSlots
            .FirstOrDefaultAsync(t => t.TimeSlotId == id);

        if (timeSlot == null)
        {
            return NotFound();
        }

        return View(timeSlot);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var timeSlot = await _context.TimeSlots.FindAsync(id);

        if (timeSlot != null)
        {
            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}