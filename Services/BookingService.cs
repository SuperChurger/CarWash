using Microsoft.EntityFrameworkCore;

namespace CarWash;

public class BookingService
{
    private const int BookingWindowHours = 12;
    private const int SlotMinutes = 15;
    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public DateTime GetNearestQuarter(DateTime now)
    {
        var localNow = AsLocal(now);
        var rounded = new DateTime(localNow.Year, localNow.Month, localNow.Day, localNow.Hour, localNow.Minute, 0);
        var mod = rounded.Minute % SlotMinutes;

        if (mod == 0 && localNow.Second == 0)
        {
            return rounded;
        }

        return rounded.AddMinutes(SlotMinutes - mod);
    }

    public async Task<List<DateTime>> GetAvailableSlotsAsync(int carWashId, DateTime now)
    {
        var slots = await GetSlotsAsync(carWashId, now);
        return slots.Where(s => !s.IsBooked).Select(s => s.StartTime).ToList();
    }

    public async Task<List<SlotItemViewModel>> GetSlotsAsync(int carWashId, DateTime now)
    {
        var from = GetNearestQuarter(now);
        var to = from.AddHours(BookingWindowHours);

        var booked = await _context.Bookings
            .Where(b => b.CarWashId == carWashId && b.StartTime >= from && b.StartTime <= to)
            .Select(b => b.StartTime)
            .ToListAsync();

        var bookedSet = booked
            .Select(ToMinuteKey)
            .ToHashSet();

        var result = new List<SlotItemViewModel>();

        for (var cursor = from; cursor <= to; cursor = cursor.AddMinutes(SlotMinutes))
        {
            result.Add(new SlotItemViewModel
            {
                StartTime = cursor,
                IsBooked = bookedSet.Contains(ToMinuteKey(cursor))
            });
        }

        return result;
    }

    public async Task<DateTime?> GetNearestAvailableSlotAsync(int carWashId, DateTime now)
    {
        var slots = await GetSlotsAsync(carWashId, now);
        var nearest = slots.FirstOrDefault(s => !s.IsBooked);
        return nearest?.StartTime;
    }

    public async Task<Booking?> GetUserActiveBookingAsync(int userId, DateTime now)
    {
        var from = GetNearestQuarter(now);
        return await _context.Bookings
            .Include(b => b.CarWash)
            .Where(b => b.UserId == userId && b.StartTime >= from)
            .OrderBy(b => b.StartTime)
            .FirstOrDefaultAsync();
    }

    public async Task<BookingAttemptStatus> TryBookAsync(int carWashId, int userId, DateTime requestedStartTime, DateTime now)
    {
        var from = GetNearestQuarter(now);
        var to = from.AddHours(BookingWindowHours);
        var slot = AsLocal(requestedStartTime);

        if (slot < from || slot > to || slot.Minute % SlotMinutes != 0 || slot.Second != 0)
        {
            return BookingAttemptStatus.OutOfRange;
        }

        var alreadyBooked = await _context.Bookings
            .AnyAsync(b => b.CarWashId == carWashId && b.StartTime == slot);

        if (alreadyBooked)
        {
            return BookingAttemptStatus.AlreadyBooked;
        }

        _context.Bookings.Add(new Booking
        {
            UserId = userId,
            CarWashId = carWashId,
            StartTime = slot,
            CreatedAt = DateTime.Now
        });

        await _context.SaveChangesAsync();
        return BookingAttemptStatus.Success;
    }

    public async Task<bool> CancelBookingAsync(int bookingId, int userId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

        if (booking == null)
        {
            return false;
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }

    private static long ToMinuteKey(DateTime value)
    {
        var local = AsLocal(value);
        return local.Year * 100000000L
               + local.Month * 1000000L
               + local.Day * 10000L
               + local.Hour * 100L
               + local.Minute;
    }

    private static DateTime AsLocal(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Local => value,
            DateTimeKind.Utc => value.ToLocalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Local)
        };
    }
}
