using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdam.Helpers;
using Sdam.Services;

namespace Sdam.ViewModels;

public partial class SubjectsViewModel(Database db) : ObservableObject
{
    public ObservableCollection<SubjectItem> Items { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasItems))]
    private bool isEmpty;

    public bool HasItems => !IsEmpty;

    [ObservableProperty] private SubjectItem? selectedItem;

    public async Task LoadAsync()
    {
        var subjects = await db.GetSubjectsAsync();
        var assignments = await db.GetAssignmentsAsync();
        var bySubject = assignments.ToLookup(a => a.SubjectId);

        Items.Clear();
        foreach (var subject in subjects)
        {
            var list = bySubject[subject.Id].ToList();
            Items.Add(new SubjectItem(subject, list.Count(a => a.IsDone), list.Count));
        }

        IsEmpty = Items.Count == 0;
    }

    partial void OnSelectedItemChanged(SubjectItem? value)
    {
        if (value is null)
            return;

        SelectedItem = null;
        _ = Shell.Current.GoToAsync($"{Routes.Subject}?id={value.Id}");
    }

    [RelayCommand]
    Task Add() => Shell.Current.GoToAsync(Routes.SubjectForm);
}
