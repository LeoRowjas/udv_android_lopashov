using System.Text;

namespace udv_anketa;

public partial class MainPage : ContentPage
{
    static readonly DateTime DefaultVisitDate = new(2026, 10, 10);
    const double DefaultRating = 5;
    const double DefaultVisits = 1;

    readonly RadioButton[] _timeRadios;

    public MainPage()
    {
        InitializeComponent();
        _timeRadios = [_morningRadio_, _dayRadio_, _eveningRadio_];
    }

    private void OnNewsToggled(object? sender, ToggledEventArgs e)
    {
        if (!e.Value)
            _emailEntry_.Text = string.Empty;
    }

    private void OnShowClicked(object? sender, EventArgs e)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(_nameEntry_.Text))
            errors.Add("укажите имя");
        if (_drinkPicker_.SelectedIndex < 0)
            errors.Add("выберите любимый напиток");

        if (errors.Count > 0)
        {
            _errorLabel_.Text = "Заполните обязательные поля: " + string.Join(", ", errors) + ".";
            _errorLabel_.IsVisible = true;
            _resultBorder_.IsVisible = false;
            return;
        }

        _errorLabel_.IsVisible = false;

        var time = _timeRadios.FirstOrDefault(r => r.IsChecked)?.Value as string ?? "не указано";
        var extras = new List<string>();
        if (_syrupCheck_.IsChecked) extras.Add("добавляю сироп");
        if (_takeawayCheck_.IsChecked) extras.Add("беру с собой");

        var sb = new StringBuilder();
        sb.AppendLine($"Имя: {_nameEntry_.Text.Trim()}");
        sb.AppendLine($"Телефон: {Fallback(_phoneEntry_.Text)}");
        sb.AppendLine(_newsSwitch_.IsToggled
            ? $"Новости на почту: да ({Fallback(_emailEntry_.Text)})"
            : "Новости на почту: нет");
        sb.AppendLine($"Напиток: {_drinkPicker_.SelectedItem}");
        sb.AppendLine($"Время визита: {time}");
        sb.AppendLine($"Обычно заказываю: {(extras.Count > 0 ? string.Join(", ", extras) : "без добавок")}");
        sb.AppendLine($"Последний визит: {_visitDatePicker_.Date:dd.MM.yyyy}");
        sb.AppendLine($"Оценка: {_ratingSlider_.Value:F0} из 10");
        sb.AppendLine($"Визитов в неделю: {_visitsStepper_.Value:F0}");
        sb.Append($"Пожелания: {Fallback(_commentEditor_.Text)}");

        _resultLabel_.Text = sb.ToString();
        _resultBorder_.IsVisible = true;
    }

    private void OnClearClicked(object? sender, EventArgs e)
    {
        _nameEntry_.Text = string.Empty;
        _phoneEntry_.Text = string.Empty;
        _newsSwitch_.IsToggled = false;
        _emailEntry_.Text = string.Empty;
        _drinkPicker_.SelectedIndex = -1;
        foreach (var radio in _timeRadios)
            radio.IsChecked = false;
        _syrupCheck_.IsChecked = false;
        _takeawayCheck_.IsChecked = false;
        _visitDatePicker_.Date = DefaultVisitDate;
        _ratingSlider_.Value = DefaultRating;
        _visitsStepper_.Value = DefaultVisits;
        _commentEditor_.Text = string.Empty;

        _errorLabel_.IsVisible = false;
        _resultLabel_.Text = string.Empty;
        _resultBorder_.IsVisible = false;
    }

    static string Fallback(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "не указано" : value.Trim();
}
