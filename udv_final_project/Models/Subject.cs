using SQLite;

namespace Sdam.Models;

[Table("subjects")]
public class Subject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Name { get; set; } = "";

    public string Teacher { get; set; } = "";

    public string ColorHex { get; set; } = SubjectColors.All[0];

    // Picker выводит предмет через ToString
    public override string ToString() => Name;
}

public static class SubjectColors
{
    public static readonly IReadOnlyList<string> All =
    [
        "#3D6FD9", "#3C8D5A", "#7A5AC8", "#C08A1E",
        "#C8361E", "#1F8A8A", "#C2477F", "#5B6470",
    ];
}
