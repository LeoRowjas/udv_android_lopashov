using Sdam.Models;
using SQLite;

namespace Sdam.Services;

public class Database
{
    const string FileName = "sdam.db3";

    readonly Lazy<Task<SQLiteAsyncConnection>> _connection = new(OpenAsync);

    static async Task<SQLiteAsyncConnection> OpenAsync()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, FileName);
        var db = new SQLiteAsyncConnection(path,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        await db.CreateTableAsync<Subject>();
        await db.CreateTableAsync<Assignment>();
        return db;
    }

    Task<SQLiteAsyncConnection> Db => _connection.Value;

    // Предметы

    public async Task<List<Subject>> GetSubjectsAsync() =>
        await (await Db).Table<Subject>().OrderBy(s => s.Name).ToListAsync();

    public async Task<Subject?> GetSubjectAsync(int id) =>
        await (await Db).FindAsync<Subject>(id);

    public async Task SaveSubjectAsync(Subject subject)
    {
        var db = await Db;
        if (subject.Id == 0)
            await db.InsertAsync(subject);
        else
            await db.UpdateAsync(subject);
    }

    public async Task DeleteSubjectAsync(int id)
    {
        var db = await Db;
        await db.RunInTransactionAsync(conn =>
        {
            conn.Execute("DELETE FROM assignments WHERE SubjectId = ?", id);
            conn.Delete<Subject>(id);
        });
    }

    // Задания

    public async Task<List<Assignment>> GetAssignmentsAsync() =>
        await (await Db).Table<Assignment>().ToListAsync();

    public async Task<List<Assignment>> GetAssignmentsAsync(bool done) =>
        await (await Db).Table<Assignment>().Where(a => a.IsDone == done).ToListAsync();

    public async Task<List<Assignment>> GetAssignmentsBySubjectAsync(int subjectId) =>
        await (await Db).Table<Assignment>().Where(a => a.SubjectId == subjectId).ToListAsync();

    public async Task<int> CountAssignmentsAsync(int subjectId) =>
        await (await Db).Table<Assignment>().Where(a => a.SubjectId == subjectId).CountAsync();

    public async Task<Assignment?> GetAssignmentAsync(int id) =>
        await (await Db).FindAsync<Assignment>(id);

    public async Task SaveAssignmentAsync(Assignment assignment)
    {
        var db = await Db;
        if (assignment.Id == 0)
            await db.InsertAsync(assignment);
        else
            await db.UpdateAsync(assignment);
    }

    public async Task DeleteAssignmentAsync(int id) =>
        await (await Db).DeleteAsync<Assignment>(id);
}
