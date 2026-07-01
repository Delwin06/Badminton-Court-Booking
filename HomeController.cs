using BadmintonCourtBookingSystem.Data;
using BadmintonCourtBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using BadmintonCourtBookingSystem.ViewModels;


namespace BadmintonCourtBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                TotalCourts = await _context.Courts.CountAsync(),

                TotalBookings = await _context.Bookings.CountAsync(),

                TotalUsers = await _context.Users.CountAsync(),

                TotalReviews = await _context.Reviews.CountAsync(),

                FeaturedCourts = await _context.Courts
                    .Include(c => c.CourtImages)
                    .Take(6)
                    .ToListAsync()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }

       
    }
}