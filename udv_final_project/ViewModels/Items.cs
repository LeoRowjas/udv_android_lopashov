using CommunityToolkit.Mvvm.ComponentModel;
using Sdam.Helpers;
using Sdam.Models;

namespace Sdam.ViewModels;

public static class Palette
{
    public static readonly Color Ink = Color.FromArgb("#141414");
    public static readonly Color Grey = Color.FromArgb("#66655F");
    public static readonly Color Rule = Color.FromArgb("#CFCCC3");
    public static readonly Color Red = Color.FromArgb("#C8361E");
    public static readonly Color Surface = Color.FromArgb("#FFFFFD");

    public static Color Parse(string? hex) =>
        Color.TryParse(hex, out var color) ? color : Grey;
}

// Строка задания в списках
public class AssignmentItem
{
    public AssignmentItem(Assignment model, Subject? subject, DateTime now)
    {
        Model = model;
        SubjectName = subject?.Name ?? "Без предмета";
        SubjectColor = model.IsDone ? Palette.Rule : Palette.Parse(subject?.ColorHex);
        IsOverdue = !model.IsDone && model.Deadline < now;
        DueText = model.IsDone && model.DoneAt is { } doneAt
            ? $"сдано {Ru.Day(doneAt)}"
            : Ru.Due(model.Deadline, now);
    }

    public Assignment Model { get; }
    public int Id => Model.Id;
    public string Title => Model.Title;
    public string SubjectName { get; }
    public Color SubjectColor { get; }
    public string Subtitle => $"{SubjectName} · {Labels.Of(Model.Type)}";
    public string DueText { get; }
    public bool IsOverdue { get; }
    public Color DueColor => IsOverdue ? Palette.Red : Palette.Grey;
    public bool IsHighPriority => !Model.IsDone && Model.Priority == Priority.High;
    public double ItemOpacity => Model.IsDone ? 0.55 : 1;
}

public class AssignmentGroup(string name, bool isOverdue, IEnumerable<AssignmentItem> items)
    : List<AssignmentItem>(items)
{
    public string Name { get; } = name;
    public Color HeaderColor { get; } = isOverdue ? Palette.Red : Palette.Grey;
}

public class SubjectItem(Subject model, int done, int total)
{
    public Subject Model { get; } = model;
    public int Id => Model.Id;
    public string Name => Model.Name;
    public string Teacher => Model.Teacher;
    public bool HasTeacher => !string.IsNullOrWhiteSpace(Model.Teacher);
    public Color Color { get; } = Palette.Parse(model.ColorHex);
    public double Progress => total == 0 ? 0 : (double)done / total;
    public string ProgressText => total == 0 ? "Заданий пока нет" : $"Сдано {done} из {total}";
}

public partial class FilterChip(int subjectId, string name, Color? color) : ObservableObject
{
    public int SubjectId { get; } = subjectId;
    public string Name { get; } = name;
    public Color DotColor { get; } = color ?? Colors.Transparent;
    public bool HasDot { get; } = color is not null;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Background), nameof(Foreground))]
    private bool isSelected;

    public Color Background => IsSelected ? Palette.Ink : Palette.Surface;
    public Color Foreground => IsSelected ? Palette.Surface : Palette.Ink;
}

public partial class ColorOption(string hex) : ObservableObject
{
    public string Hex { get; } = hex;
    public Color Color { get; } = Color.FromArgb(hex);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Ring))]
    private bool isSelected;

    public Color Ring => IsSelected ? Palette.Ink : Colors.Transparent;
}
