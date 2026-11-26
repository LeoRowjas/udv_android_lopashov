using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdam.Helpers;
using Sdam.Models;
using Sdam.Services;

namespace Sdam.ViewModels;

public partial class SubjectDetailViewModel(Database db) : ObservableObject, IQueryAttributable
{
    int _id;
    Subject? _subject;

    public ObservableCollection<AssignmentItem> Items { get; } = [];

    [ObservableProperty] private string name = "";
    [ObservableProperty] private string teacher = "";
    [ObservableProperty] private bool hasTeacher;
    [ObservableProperty] private Color color = Palette.Grey;
    [ObservableProperty] private double progress;
    [ObservableProperty] private string progressText = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasItems))]
    private bool isEmpty;

    public bool HasItems => !IsEmpty;

    [ObservableProperty] private AssignmentItem? selectedItem;

    public void ApplyQueryAttributes(IDictionary<string, object> query) =>
        _id = Routes.GetInt(query, "id");

    public async Task LoadAsync()
    {
        _subject = await db.GetSubjectAsync(_id);
        if (_subject is null)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        var assignments = await db.GetAssignmentsBySubjectAsync(_id);
        var summary = new SubjectItem(_subject, assignments.Count(a => a.IsDone), assignments.Count);
        var now = DateTime.Now;

        Name = _subject.Name;
        Teacher = _subject.Teacher;
        HasTeacher = summary.HasTeacher;
        Color = summary.Color;
        Progress = summary.Progress;
        ProgressText = summary.ProgressText;

        Items.Clear();
        // Сначала несданные по сроку, потом сданные — последние сверху
        var ordered = assignments.Where(a => !a.IsDone).OrderBy(a => a.Deadline)
            .Concat(assignments.Where(a => a.IsDone).OrderByDescending(a => a.DoneAt));
        foreach (var assignment in ordered)
            Items.Add(new AssignmentItem(assignment, _subject, now));

        IsEmpty = Items.Count == 0;
    }

    partial void OnSelectedItemChanged(AssignmentItem? value)
    {
        if (value is null)
            return;

        SelectedItem = null;
        _ = Shell.Current.GoToAsync($"{Routes.Assignment}?id={value.Id}");
    }

    [RelayCommand]
    Task AddAssignment() => Shell.Current.GoToAsync($"{Routes.AssignmentForm}?subjectId={_id}");

    [RelayCommand]
    Task Edit() => Shell.Current.GoToAsync($"{Routes.SubjectForm}?id={_id}");

    [RelayCommand]
    async Task Delete()
    {
        if (_subject is null)
            return;

        var count = await db.CountAssignmentsAsync(_id);
        var message = count == 0
            ? $"Предмет «{_subject.Name}» будет удалён."
            : $"Вместе с предметом «{_subject.Name}» удалятся {count} {Ru.Plural(count, "задание", "задания", "заданий")}.";

        if (!await Dialogs.ConfirmAsync("Удалить предмет?", message, "Удалить"))
            return;

        try
        {
            await db.DeleteSubjectAsync(_id);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            await Dialogs.ErrorAsync("Не удалось удалить предмет. Попробуйте ещё раз.");
        }
    }
}
