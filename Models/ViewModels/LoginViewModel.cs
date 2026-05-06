using System.ComponentModel.DataAnnotations;

namespace CarWash;

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите логин")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; } = string.Empty;

    // Сдвиг времени при входе (формат 1:30 / -1:30) — отключено.
    // [RegularExpression(@"^-?\d{1,2}:[0-5]\d$", ErrorMessage = "Формат сдвига: 1:30 или -1:30")]
    // public string? TimeShift { get; set; }
}
