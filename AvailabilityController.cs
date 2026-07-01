using BadmintonCourtBookingSystem.Data;
using BadmintonCourtBookingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BadmintonCourtBookingSystem.Controllers
{
    public class AvailabilityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            DateOnly? bookingDate,
            int? timeSlotId)
        {
            var vm = new CourtAvailabilityViewModel
            {
                BookingDate = bookingDate ??
                              DateOnly.FromDateTime(DateTime.Today),

                TimeSlotId = timeSlotId,

                TimeSlots = await _context.TimeSlots
                    .Select(ts => new SelectListItem
                    {
                        Value = ts.TimeSlotId.ToString(),
                        Text = ts.StartTime + " - " + ts.EndTime
                    })
                    .ToListAsync()
            };

            if (timeSlotId.HasValue)
            {
                var bookedCourtIds = await _context.Bookings
                    .Where(b =>
                        b.BookingDate == vm.BookingDate &&
                        b.TimeSlotId == vm.TimeSlotId &&
                        b.BookingStatus != "Cancelled")
                    .Select(b => b.CourtId)
                    .ToListAsync();

                vm.AvailableCourts = await _context.Courts
                    .Include(c => c.CourtImages)
                    .Where(c => !bookedCourtIds.Contains(c.CourtId))
                    .ToListAsync();
            }

            return View(vm);
        }
    }
}