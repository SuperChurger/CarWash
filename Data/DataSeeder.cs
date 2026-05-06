using Microsoft.EntityFrameworkCore;

namespace CarWash;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Users.AnyAsync())
        {
            db.Users.AddRange(
                new User { Login = "ivan", Password = "1234" },
                new User { Login = "olga", Password = "1234" });
        }

        await SyncCarWashesAsync(db);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Приводит список моек к каноническому виду (названия/адреса), чтобы после смены строки подключения
    /// или старых данных в БД в интерфейсе отображались актуальные значения.
    /// </summary>
    public static async Task SyncCarWashesAsync(AppDbContext db)
    {
        var existing = await db.CarWashes.OrderBy(c => c.Id).ToListAsync();

        for (var i = 0; i < CarWashCatalog.Items.Length; i++)
        {
            var (name, address) = CarWashCatalog.Items[i];
            if (i < existing.Count)
            {
                existing[i].Name = name;
                existing[i].Address = address;
            }
            else
            {
                db.CarWashes.Add(new CarWash { Name = name, Address = address });
            }
        }
    }
}
