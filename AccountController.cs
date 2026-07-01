using BadmintonCourtBookingSystem.Data;
using BadmintonCourtBookingSystem.Models;
using BadmintonCourtBookingSystem.Services.Interfaces;
using BadmintonCourtBookingSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace BadmintonCourtBookingSystem.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordService _passwordService;

    public AccountController(
        ApplicationDbContext context,
        IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        bool emailExists = await _context.Users
            .AnyAsync(x => x.Email == model.Email);

        if (emailExists)
        {
            ModelState.AddModelError("", "Email already exists");
            return View(model);
        }

        int customerRoleId = await _context.Roles
            .Where(r => r.RoleName == "Customer")
            .Select(r => r.RoleId)
            .FirstAsync();

        User user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            PasswordHash = _passwordService.HashPassword(model.Password),
            RoleId = customerRoleId,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        bool isValidPassword =
            _passwordService.VerifyPassword(
                model.Password,
                user.PasswordHash);

        if (!isValidPassword)
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier,
            user.UserId.ToString()),

        new Claim(ClaimTypes.Name,
            $"{user.FirstName} {user.LastName}"),

        new Claim(ClaimTypes.Email,
            user.Email),

        new Claim(ClaimTypes.Role,
            user.Role!.RoleName)
    };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
    CookieAuthenticationDefaults.AuthenticationScheme,
    principal);

        if (user.Role?.RoleName == "Admin" ||
            user.Role?.RoleName == "Staff")
        {
            return RedirectToAction("Index", "Dashboard");
        }

        return RedirectToAction("Index", "Home");
    }
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GenerateAdminHash()
    {
        return Content(_passwordService.HashPassword("Admin@123"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }




}