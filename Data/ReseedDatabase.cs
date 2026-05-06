using Microsoft.EntityFrameworkCore;

namespace CarWash;

/// <summary>
/// Полная очистка данных и повторное наполнение тестовыми записями.
/// Включается в appsettings: "Database": { "ReseedOnStartup": true } (лучше только в Development).
/// </summary>
public static class ReseedDatabase
{
    public static async Task ClearAndSeedTestDataAsync(AppDbContext db)
    {
        await db.Bookings.ExecuteDeleteAsync();
        await db.Users.ExecuteDeleteAsync();
        await db.CarWashes.ExecuteDeleteAsync();

        db.Users.AddRange(
            new User { Login = "ivan", Password = "1234" },
            new User { Login = "olga", Password = "1234" });

        foreach (var (name, address) in CarWashCatalog.Items)
        {
            db.CarWashes.Add(new CarWash { Name = name, Address = address });
        }

        await db.SaveChangesAsync();
    }
}
