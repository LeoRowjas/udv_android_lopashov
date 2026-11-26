using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdam.Helpers;
using Sdam.Models;
using Sdam.Services;

namespace Sdam.ViewModels;

public partial class SubjectFormViewModel(Database db) : ObservableObject, IQueryAttributable
{
    int _id;
    bool _loaded;
    bool _nameTouched;
    Subject? _existing;

    public ObservableCollection<ColorOption> ColorOptions { get; } =
        new(SubjectColors.All.Select(hex => new ColorOption(hex)));

    [ObservableProperty] private string pageTitle = "Новый предмет";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowNameError))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string name = "";

    public bool ShowNameError => _nameTouched && string.IsNullOrWhiteSpace(Name);

    [ObservableProperty] private string teacher = "";
    [ObservableProperty] private ColorOption? selectedColor;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private bool isSaving;

    public void ApplyQueryAttributes(IDictionary<string, object> query) =>
        _id = Routes.GetInt(query, "id");

    public async Task LoadAsync()
    {
        if (_loaded)
            return;

        var hex = SubjectColors.All[0];
        if (_id != 0 && await db.GetSubjectAsync(_id) is { } existing)
        {
            _existing = existing;
            PageTitle = "Изменить предмет";
            Name = existing.Name;
            Teacher = existing.Teacher;
            hex = existing.ColorHex;
        }
        else
        {
            // Новому предмету даём цвет, который ещё не занят
            var used = (await db.GetSubjectsAsync()).Select(s => s.ColorHex).ToHashSet();
            hex = SubjectColors.All.FirstOrDefault(c => !used.Contains(c)) ?? hex;
        }

        SelectedColor = ColorOptions.FirstOrDefault(o => o.Hex == hex) ?? ColorOptions[0];
        _loaded = true;
    }

    partial void OnNameChanged(string value)
    {
        if (_loaded)
            _nameTouched = true;
    }

    partial void OnSelectedColorChanged(ColorOption? value)
    {
        foreach (var option in ColorOptions)
            option.IsSelected = option == value;
    }

    bool CanSave() => !string.IsNullOrWhiteSpace(Name) && !IsSaving;

    [RelayCommand(CanExecute = nameof(CanSave))]
    async Task Save()
    {
        var subject = _existing ?? new Subject();
        subject.Name = Name.Trim();
        subject.Teacher = Teacher.Trim();
        subject.ColorHex = SelectedColor?.Hex ?? SubjectColors.All[0];

        IsSaving = true;
        try
        {
            await db.SaveSubjectAsync(subject);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception)
        {
            await Dialogs.ErrorAsync("Не удалось сохранить предмет. Попробуйте ещё раз.");
        }
        finally
        {
            IsSaving = false;
        }
    }
}
