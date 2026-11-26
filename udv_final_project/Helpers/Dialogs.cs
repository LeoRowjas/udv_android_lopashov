namespace Sdam.Helpers;

public static class Dialogs
{
    static Page Page => Shell.Current;

    public static Task<bool> ConfirmAsync(string title, string message, string accept) =>
        Page.DisplayAlertAsync(title, message, accept, "Отмена");

    public static Task ErrorAsync(string message) =>
        Page.DisplayAlertAsync("Ошибка", message, "Понятно");
}
