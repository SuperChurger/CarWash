namespace CarWash;

public class CarWashListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime? NearestSlot { get; set; }
}
