using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdam.Helpers;
using Sdam.Models;
using Sdam.Services;

namespace Sdam.ViewModels;

public partial class DeadlinesViewModel(Database db) : ObservableObject
{
    List<Subject> _subjects = [];
    List<Assignment> _assignments = [];
    int _filterSubjectId;

    public ObservableCollection<AssignmentGroup> Groups { get; } = [];
    public ObservableCollection<FilterChip> Filters { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasItems))]
    private bool isEmpty;

    public bool HasItems => !IsEmpty;

    [ObservableProperty] private bool hasSubjects;
    [ObservableProperty] private string emptyTitle = "";
    [ObservableProperty] private string emptyText = "";
    [ObservableProperty] private string emptyButton = "";
    [ObservableProperty] private AssignmentItem? selectedItem;
    [ObservableProperty] private FilterChip? selectedFilter;

    public async Task LoadAsync()
    {
        _subjects = await db.GetSubjectsAsync();
        _assignments = await db.GetAssignmentsAsync(done: false);
        HasSubjects = _subjects.Count > 0;

        if (_subjects.All(s => s.Id != _filterSubjectId))
            _filterSubjectId = 0;

        BuildFilters();
        BuildGroups();
    }

    void BuildFilters()
    {
        Filters.Clear();
        Filters.Add(new FilterChip(0, "Все", null));
        foreach (var subject in _subjects)
            Filters.Add(new FilterChip(subject.Id, subject.Name, Palette.Parse(subject.ColorHex)));

        var current = Filters.First(f => f.SubjectId == _filterSubjectId);
        current.IsSelected = true;
        SelectedFilter = current;
    }

    void BuildGroups()
    {
        var now = DateTime.Now;
        var today = now.Date;
        var subjects = _subjects.ToDictionary(s => s.Id);

        var items = _assignments
            .Where(a => _filterSubjectId == 0 || a.SubjectId == _filterSubjectId)
            .OrderBy(a => a.Deadline)
            .Select(a => new AssignmentItem(a, subjects.GetValueOrDefault(a.SubjectId), now))
            .ToList();

        Groups.Clear();
        AddGroup("Просрочено", true, items.Where(i => i.IsOverdue));
        AddGroup("Сегодня", false, items.Where(i => !i.IsOverdue && i.Model.Deadline.Date == today));
        AddGroup("Завтра", false, items.Where(i => i.Model.Deadline.Date == today.AddDays(1)));
        AddGroup("На неделе", false, items.Where(i =>
            i.Model.Deadline.Date > today.AddDays(1) && i.Model.Deadline.Date <= today.AddDays(7)));
        AddGroup("Позже", false, items.Where(i => i.Model.Deadline.Date > today.AddDays(7)));

        IsEmpty = items.Count == 0;
        if (!HasSubjects)
        {
            EmptyTitle = "Пока нет предметов";
            EmptyText = "Добавьте предмет, а потом записывайте по нему задания";
            EmptyButton = "Добавить предмет";
        }
        else if (_filterSubjectId != 0)
        {
            EmptyTitle = "По этому предмету всё сдано";
            EmptyText = "Несданных заданий по предмету нет";
            EmptyButton = "Новое задание";
        }
        else
        {
            EmptyTitle = "Несданных заданий нет";
            EmptyText = "Когда появится новое задание, запишите его сюда";
            EmptyButton = "Новое задание";
        }
    }

    void AddGroup(string name, bool overdue, IEnumerable<AssignmentItem> items)
    {
        var group = new AssignmentGroup(name, overdue, items);
        if (group.Count > 0)
            Groups.Add(group);
    }

    partial void OnSelectedFilterChanged(FilterChip? value)
    {
        if (value is null || value.SubjectId == _filterSubjectId)
            return;

        _filterSubjectId = value.SubjectId;
        foreach (var chip in Filters)
            chip.IsSelected = chip.SubjectId == _filterSubjectId;
        BuildGroups();
    }

    partial void OnSelectedItemChanged(AssignmentItem? value)
    {
        if (value is null)
            return;

        SelectedItem = null;
        _ = Shell.Current.GoToAsync($"{Routes.Assignment}?id={value.Id}");
    }

    [RelayCommand]
    Task Add() => _filterSubjectId == 0
        ? Shell.Current.GoToAsync(Routes.AssignmentForm)
        : Shell.Current.GoToAsync($"{Routes.AssignmentForm}?subjectId={_filterSubjectId}");

    [RelayCommand]
    Task EmptyAction() => HasSubjects ? Add() : Shell.Current.GoToAsync(Routes.SubjectForm);
}
