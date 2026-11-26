using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdam.Helpers;
using Sdam.Models;
using Sdam.Services;

namespace Sdam.ViewModels;

public partial class AssignmentDetailViewModel(Database db) : ObservableObject, IQueryAttributable
{
    int _id;
    Assignment? _assignment;

    [ObservableProperty] private string title = "";
    [ObservableProperty] private string subjectName = "";
    [ObservableProperty] private Color subjectColor = Palette.Grey;
    [ObservableProperty] private string typeName = "";
    [ObservableProperty] private string deadlineText = "";
    [ObservableProperty] private Color deadlineColor = Palette.Ink;
    [ObservableProperty] private string priorityName = "";
    [ObservableProperty] private string note = "";
    [ObservableProperty] private bool hasNote;
    [ObservableProperty] private string statusText = "";
    [ObservableProperty] private Color statusColor = Palette.Grey;
    [ObservableProperty] private string doneButtonText = "";

    public void ApplyQueryAttributes(IDictionary<string, object> query) =>
        _id = Routes.GetInt(query, "id");

    public async Task LoadAsync()
    {
        _assignment = await db.GetAssignmentAsync(_id);
        if (_assignment is null)
        {
            // Задание удалили на другом экране
            await Shell.Current.GoToAsync("..");
            return;
        }

        var a = _assignment;
        var subject = await db.GetSubjectAsync(a.SubjectId);
        var overdue = !a.IsDone && a.Deadline < DateTime.Now;

        Title = a.Title;
        SubjectName = subject?.Name ?? "Без предмета";
        SubjectColor = Palette.Parse(subject?.ColorHex);
        TypeName = Labels.Of(a.Type);
        DeadlineText = Ru.Full(a.Deadline);
        DeadlineColor = overdue ? Palette.Red : Palette.Ink;
        PriorityName = Labels.Of(a.Priority);
        Note = a.Note ?? "";
        HasNote = !string.IsNullOrWhiteSpace(a.Note);

        if (a.IsDone)
        {
            StatusText = a.DoneAt is { } doneAt ? $"Сдано {Ru.Day(doneAt)}" : "Сдано";
            StatusColor = Palette.Grey;
            DoneButtonText = "Вернуть в работу";
        }
        else
        {
            StatusText = overdue ? "Просрочено" : "Не сдано";
            StatusColor = overdue ? Palette.Red : Palette.Ink;
            DoneButtonText = "Сдано";
        }
    }

    [RelayCommand]
    async Task ToggleDone()
    {
        if (_assignment is null)
            return;

        _assignment.IsDone = !_assignment.IsDone;
        _assignment.DoneAt = _assignment.IsDone ? DateTime.Now : null;
        try
        {
            await db.SaveAssignmentAsync(_assignment);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            _assignment.IsDone = !_assignment.IsDone;
            await Dialogs.ErrorAsync("Не удалось сохранить изменения. Попробуйте ещё раз.");
        }
    }

    [RelayCommand]
    Task Edit() => Shell.Current.GoToAsync($"{Routes.AssignmentForm}?id={_id}");

    [RelayCommand]
    async Task Delete()
    {
        if (_assignment is null)
            return;

        var ok = await Dialogs.ConfirmAsync("Удалить задание?", $"«{_assignment.Title}» будет удалено.", "Удалить");
        if (!ok)
            return;

        try
        {
            await db.DeleteAssignmentAsync(_id);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            await Dialogs.ErrorAsync("Не удалось удалить задание. Попробуйте ещё раз.");
        }
    }
}
