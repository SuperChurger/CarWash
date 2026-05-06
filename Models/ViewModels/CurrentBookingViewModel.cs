namespace CarWash;

public class CurrentBookingViewModel
{
    public int BookingId { get; set; }
    public string CarWashName { get; set; } = string.Empty;
    public string CarWashAddress { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
}
