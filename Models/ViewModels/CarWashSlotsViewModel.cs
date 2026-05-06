namespace CarWash;

public class CarWashSlotsViewModel
{
    public int CarWashId { get; set; }
    public string CarWashName { get; set; } = string.Empty;
    public string CarWashAddress { get; set; } = string.Empty;
    public List<SlotItemViewModel> Slots { get; set; } = new();
}
