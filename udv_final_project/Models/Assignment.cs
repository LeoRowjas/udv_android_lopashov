using SQLite;

namespace Sdam.Models;

[Table("assignments")]
public class Assignment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int SubjectId { get; set; }

    [NotNull]
    public string Title { get; set; } = "";

    public AssignmentType Type { get; set; }

    public DateTime Deadline { get; set; }

    public Priority Priority { get; set; } = Priority.Medium;

    public string? Note { get; set; }

    public bool IsDone { get; set; }

    public DateTime? DoneAt { get; set; }
}

public enum AssignmentType
{
    Lab,
    Homework,
    Coursework,
    Test,
    Exam,
    Other,
}

public enum Priority
{
    Low,
    Medium,
    High,
}

public static class Labels
{
    // Порядок совпадает с порядком значений в enum
    public static readonly IReadOnlyList<string> Types =
        ["Лабораторная", "Домашнее задание", "Курсовая", "Контрольная", "Экзамен", "Другое"];

    public static readonly IReadOnlyList<string> Priorities = ["Низкий", "Средний", "Высокий"];

    public static string Of(AssignmentType type) => Types[(int)type];

    public static string Of(Priority priority) => Priorities[(int)priority];
}
