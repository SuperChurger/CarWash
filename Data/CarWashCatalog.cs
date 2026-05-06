namespace CarWash;

/// <summary>Канонический список моек для сидирования и синхронизации с БД.</summary>
public static class CarWashCatalog
{
    public static readonly (string Name, string Address)[] Items =
    {
        ("Майская", "Майская, 51"),
        ("Песочная", "Песочная улица, 38е"),
        ("Автозаводская", "Автозаводская, 1Б")
    };
}
