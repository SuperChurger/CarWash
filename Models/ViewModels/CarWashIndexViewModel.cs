namespace CarWash;

public class CarWashIndexViewModel
{
    public List<CarWashListItemViewModel> CarWashes { get; set; } = new();
    public CurrentBookingViewModel? CurrentBooking { get; set; }
}
