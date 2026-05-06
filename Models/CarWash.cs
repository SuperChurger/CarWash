using System.ComponentModel.DataAnnotations;

namespace CarWash;

public class CarWash
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    public List<Booking> Bookings { get; set; } = new();
}