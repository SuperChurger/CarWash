using System.ComponentModel.DataAnnotations;

namespace CarWash;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Login { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public List<Booking> Bookings { get; set; } = new();
}