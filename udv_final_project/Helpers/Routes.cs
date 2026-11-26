namespace Sdam.Helpers;

public static class Routes
{
    public const string Assignment = "assignment";
    public const string AssignmentForm = "assignment-form";
    public const string Subject = "subject";
    public const string SubjectForm = "subject-form";

    public static int GetInt(IDictionary<string, object> query, string key) =>
        query.TryGetValue(key, out var value) && int.TryParse(value?.ToString(), out var result) ? result : 0;
}
