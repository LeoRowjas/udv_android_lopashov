using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdam.Helpers;
using Sdam.Models;
using Sdam.Services;

namespace Sdam.ViewModels;

public partial class AssignmentFormViewModel(Database db) : ObservableObject, IQueryAttributable
{
    int _id;
    int _presetSubjectId;
    bool _loaded;
    bool _titleTouched;
    Assignment? _existing;

    public ObservableCollection<Subject> Subjects { get; } = [];
    public IReadOnlyList<string> Types => Labels.Types;
    public IReadOnlyList<string> Priorities => Labels.Priorities;

    [ObservableProperty] private string pageTitle = "Новое задание";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowNoSubjects))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private bool hasSubjects = true;

    public bool ShowNoSubjects => !HasSubjects;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private Subject? selectedSubject;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowTitleError))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string title = "";

    public bool ShowTitleError => _titleTouched && string.IsNullOrWhiteSpace(Title);

    [ObservableProperty] private int typeIndex;
    [ObservableProperty] private int priorityIndex = (int)Priority.Medium;
    [ObservableProperty] private string note = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowPastWarning))]
    private DateTime? date = DateTime.Today.AddDays(1);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowPastWarning))]
    private TimeSpan? time = new TimeSpan(23, 59, 0);

    public bool ShowPastWarning => Deadline < DateTime.Now;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private bool isSaving;

    DateTime Deadline => (Date ?? DateTime.Today).Date + (Time ?? TimeSpan.Zero);

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _id = Routes.GetInt(query, "id");
        _presetSubjectId = Routes.GetInt(query, "subjectId");
    }

    public async Task LoadAsync()
    {
        var subjects = await db.GetSubjectsAsync();
        var selectedId = SelectedSubject?.Id ?? _presetSubjectId;

        Subjects.Clear();
        foreach (var subject in subjects)
            Subjects.Add(subject);
        HasSubjects = Subjects.Count > 0;

        // Повторное появление (например, вернулись после создания предмета): поля формы не трогаем
        if (_loaded)
        {
            SelectedSubject = Subjects.FirstOrDefault(s => s.Id == selectedId) ?? Subjects.FirstOrDefault();
            return;
        }
        _loaded = true;

        if (_id != 0 && await db.GetAssignmentAsync(_id) is { } existing)
        {
            _existing = existing;
            PageTitle = "Изменить задание";
            selectedId = existing.SubjectId;
            Title = existing.Title;
            TypeIndex = (int)existing.Type;
            PriorityIndex = (int)existing.Priority;
            Date = existing.Deadline.Date;
            Time = existing.Deadline.TimeOfDay;
            Note = existing.Note ?? "";
        }

        SelectedSubject = Subjects.FirstOrDefault(s => s.Id == selectedId) ?? Subjects.FirstOrDefault();
    }

    partial void OnTitleChanged(string value)
    {
        if (_loaded)
            _titleTouched = true;
    }

    bool CanSave() =>
        HasSubjects && SelectedSubject is not null && !string.IsNullOrWhiteSpace(Title) && !IsSaving;

    [RelayCommand(CanExecute = nameof(CanSave))]
    async Task Save()
    {
        var assignment = _existing ?? new Assignment();
        assignment.SubjectId = SelectedSubject!.Id;
        assignment.Title = Title.Trim();
        assignment.Type = (AssignmentType)Math.Max(TypeIndex, 0);
        assignment.Priority = (Priority)Math.Max(PriorityIndex, 0);
        assignment.Deadline = Deadline;
        assignment.Note = string.IsNullOrWhiteSpace(Note) ? null : Note.Trim();

        IsSaving = true;
        try
        {
            await db.SaveAssignmentAsync(assignment);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            // Введённые данные остаются в форме
            await Dialogs.ErrorAsync("Не удалось сохранить задание. Попробуйте ещё раз.");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    Task AddSubject() => Shell.Current.GoToAsync(Routes.SubjectForm);
}
