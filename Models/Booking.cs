namespace CarWash;

public class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public int CarWashId { get; set; }
    public CarWash CarWash { get; set; } = default!;

    public DateTime StartTime { get; set; }

    public DateTime CreatedAt { get; set; }
}