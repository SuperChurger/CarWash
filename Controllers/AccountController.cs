using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace CarWash;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
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
        {
            return View(model);
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return View(model);
        }

        // Сдвиг «текущего времени» при логине (claim TimeShiftMinutes) — отключено.
        // var shiftMinutes = 0;
        // if (!string.IsNullOrWhiteSpace(model.TimeShift) && !TryParseTimeShift(model.TimeShift, out shiftMinutes))
        // {
        //     ModelState.AddModelError(nameof(model.TimeShift), "Некорректный формат сдвига.");
        //     return View(model);
        // }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim("UserId", user.Id.ToString())
            // , new Claim("TimeShiftMinutes", shiftMinutes.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Cookies");

        await HttpContext.SignInAsync("Cookies",
            new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "CarWash");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Login");
    }

    // private static bool TryParseTimeShift(string value, out int minutes)
    // {
    //     minutes = 0;
    //     var trimmed = value.Trim();
    //     var sign = 1;
    //     if (trimmed.StartsWith("-"))
    //     {
    //         sign = -1;
    //         trimmed = trimmed[1..];
    //     }
    //     var parts = trimmed.Split(':', StringSplitOptions.TrimEntries);
    //     if (parts.Length != 2
    //         || !int.TryParse(parts[0], out var hours)
    //         || !int.TryParse(parts[1], out var mins)
    //         || mins < 0 || mins > 59)
    //     {
    //         return false;
    //     }
    //     minutes = sign * (hours * 60 + mins);
    //     return true;
    // }
}