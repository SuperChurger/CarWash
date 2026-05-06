using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarWash;

[Authorize]
public class CarWashController : Controller
{
    private readonly AppDbContext _context;
    private readonly BookingService _bookingService;

    public CarWashController(AppDbContext context, BookingService bookingService)
    {
        _context = context;
        _bookingService = bookingService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);
        var now = DateTime.Now; // GetUserNow();
        var washes = await _context.CarWashes
            .OrderBy(c => c.Id)
            .ToListAsync();

        var washModels = new List<CarWashListItemViewModel>();
        foreach (var wash in washes)
        {
            var nearestSlot = await _bookingService.GetNearestAvailableSlotAsync(wash.Id, now);
            washModels.Add(new CarWashListItemViewModel
            {
                Id = wash.Id,
                Name = wash.Name,
                Address = wash.Address,
                NearestSlot = nearestSlot
            });
        }

        var currentBooking = await _bookingService.GetUserActiveBookingAsync(userId, now);
        var model = new CarWashIndexViewModel
        {
            CarWashes = washModels,
            CurrentBooking = currentBooking == null ? null : new CurrentBookingViewModel
            {
                BookingId = currentBooking.Id,
                CarWashName = currentBooking.CarWash.Name,
                CarWashAddress = currentBooking.CarWash.Address,
                StartTime = currentBooking.StartTime
            }
        };

        return View(model);
    }

    public async Task<IActionResult> Slots(int id)
    {
        var now = DateTime.Now; // GetUserNow();
        var wash = await _context.CarWashes.FirstOrDefaultAsync(c => c.Id == id);
        if (wash == null)
        {
            return NotFound();
        }

        var slots = await _bookingService.GetSlotsAsync(id, now);
        var model = new CarWashSlotsViewModel
        {
            CarWashId = wash.Id,
            CarWashName = wash.Name,
            CarWashAddress = wash.Address,
            Slots = slots
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Book(int carWashId, long startTicks)
    {
        var now = DateTime.Now; // GetUserNow();
        var startTime = new DateTime(startTicks, DateTimeKind.Local);
        var userId = int.Parse(User.Claims
            .First(c => c.Type == "UserId").Value);

        var existingBooking = await _bookingService.GetUserActiveBookingAsync(userId, now);
        if (existingBooking != null)
        {
            TempData["Error"] = "У вас уже есть активная запись. Сначала отмените ее на странице выбора адреса.";
            return RedirectToAction("Slots", new { id = carWashId });
        }

        var status = await _bookingService.TryBookAsync(carWashId, userId, startTime, now);
        if (status == BookingAttemptStatus.AlreadyBooked)
        {
            TempData["Error"] = "Это время уже занято.";
            return RedirectToAction("Slots", new { id = carWashId });
        }

        if (status == BookingAttemptStatus.OutOfRange)
        {
            TempData["Error"] = "Выбрано время вне окна записи (доступны только ближайшие 12 часов).";
            return RedirectToAction("Slots", new { id = carWashId });
        }

        TempData["Success"] = "Вы успешно записались.";
        return RedirectToAction("Slots", new { id = carWashId });
    }

    [HttpPost]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        var userId = int.Parse(User.Claims
            .First(c => c.Type == "UserId").Value);

        var canceled = await _bookingService.CancelBookingAsync(bookingId, userId);
        TempData[canceled ? "Success" : "Error"] = canceled
            ? "Запись отменена."
            : "Не удалось отменить запись.";

        return RedirectToAction("Index");
    }

    // Сдвиг времени из claim TimeShiftMinutes при логине — отключено.
    // private DateTime GetUserNow()
    // {
    //     var raw = User.Claims.FirstOrDefault(c => c.Type == "TimeShiftMinutes")?.Value;
    //     if (int.TryParse(raw, out var shiftMinutes))
    //     {
    //         return DateTime.Now.AddMinutes(shiftMinutes);
    //     }
    //     return DateTime.Now;
    // }
}