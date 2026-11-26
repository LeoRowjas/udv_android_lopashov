using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Sdam.Helpers;
using Sdam.Services;

namespace Sdam.ViewModels;

public partial class DoneViewModel(Database db) : ObservableObject
{
    public ObservableCollection<AssignmentItem> Items { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasItems))]
    private bool isEmpty;

    public bool HasItems => !IsEmpty;

    [ObservableProperty] private AssignmentItem? selectedItem;

    public async Task LoadAsync()
    {
        var subjects = (await db.GetSubjectsAsync()).ToDictionary(s => s.Id);
        var done = await db.GetAssignmentsAsync(done: true);
        var now = DateTime.Now;

        Items.Clear();
        foreach (var assignment in done.OrderByDescending(a => a.DoneAt))
            Items.Add(new AssignmentItem(assignment, subjects.GetValueOrDefault(assignment.SubjectId), now));

        IsEmpty = Items.Count == 0;
    }

    partial void OnSelectedItemChanged(AssignmentItem? value)
    {
        if (value is null)
            return;

        SelectedItem = null;
        _ = Shell.Current.GoToAsync($"{Routes.Assignment}?id={value.Id}");
    }
}
