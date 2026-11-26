using System.Globalization;

namespace Sdam.Helpers;

public static class Ru
{
    public static readonly CultureInfo Culture = new("ru-RU");

    public static string Plural(int n, string one, string few, string many)
    {
        var mod10 = n % 10;
        var mod100 = n % 100;
        if (mod10 == 1 && mod100 != 11) return one;
        if (mod10 is >= 2 and <= 4 && mod100 is < 12 or > 14) return few;
        return many;
    }

    // Короткий срок для строки списка: «сегодня, 23:59», «пт, 18:00», «12 дек.»
    public static string Due(DateTime deadline, DateTime now)
    {
        var days = (deadline.Date - now.Date).Days;
        var time = deadline.ToString("HH:mm", Culture);
        return days switch
        {
            0 => $"сегодня, {time}",
            1 => $"завтра, {time}",
            -1 => $"вчера, {time}",
            > 1 and < 7 => $"{deadline.ToString("ddd", Culture)}, {time}",
            _ => deadline.ToString(deadline.Year == now.Year ? "d MMM" : "d MMM yyyy", Culture),
        };
    }

    public static string Full(DateTime date) => date.ToString("dddd, d MMMM yyyy, HH:mm", Culture);

    public static string Day(DateTime date) => date.ToString("d MMMM", Culture);
}
