using Microsoft.Data.Sqlite;
using System.Data;

namespace TeachersToolbox.Data;

public class DbContext : IDisposable
{
    private readonly string _connectionString;
    private SqliteConnection? _connection;

    public DbContext(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public async Task<IDbConnection> GetConnectionAsync()
    {
        if (_connection == null)
        {
            _connection = new SqliteConnection(_connectionString);
            await _connection.OpenAsync();
        }
        return _connection;
    }

    public async Task InitializeDatabaseAsync()
    {
        using var connection = CreateConnection();
        await ((SqliteConnection)connection).OpenAsync();

        var createTablesSql = @"
            CREATE TABLE IF NOT EXISTS Classes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Grade TEXT,
                Year INTEGER,
                Teacher TEXT,
                CreatedAt TEXT DEFAULT (datetime('now', 'localtime'))
            );

            CREATE TABLE IF NOT EXISTS Students (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                StudentNumber TEXT,
                ClassId INTEGER,
                SeatNumber INTEGER,
                Gender TEXT,
                Phone TEXT,
                ParentPhone TEXT,
                Note TEXT,
                CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
                FOREIGN KEY (ClassId) REFERENCES Classes(Id)
            );

            CREATE TABLE IF NOT EXISTS Scores (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId INTEGER,
                Subject TEXT,
                Value REAL,
                MaxValue REAL DEFAULT 100,
                ExamType TEXT,
                ExamName TEXT,
                ExamDate TEXT,
                Remark TEXT,
                CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
                FOREIGN KEY (StudentId) REFERENCES Students(Id)
            );

            CREATE TABLE IF NOT EXISTS Assignments (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ClassId INTEGER,
                Title TEXT,
                Description TEXT,
                Subject TEXT,
                DueDate TEXT,
                CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
                FOREIGN KEY (ClassId) REFERENCES Classes(Id)
            );

            CREATE TABLE IF NOT EXISTS AssignmentRecords (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AssignmentId INTEGER,
                StudentId INTEGER,
                IsCompleted INTEGER DEFAULT 0,
                Score INTEGER DEFAULT 0,
                Remark TEXT,
                CompletedAt TEXT,
                FOREIGN KEY (AssignmentId) REFERENCES Assignments(Id),
                FOREIGN KEY (StudentId) REFERENCES Students(Id)
            );

            CREATE TABLE IF NOT EXISTS Courses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ClassId INTEGER,
                Subject TEXT,
                Teacher TEXT,
                Day INTEGER,
                Period INTEGER,
                Room TEXT,
                FOREIGN KEY (ClassId) REFERENCES Classes(Id)
            );

            CREATE TABLE IF NOT EXISTS CourseSchedules (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ClassId INTEGER,
                Name TEXT,
                FOREIGN KEY (ClassId) REFERENCES Classes(Id)
            );

            CREATE TABLE IF NOT EXISTS Attendances (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId INTEGER,
                Date TEXT,
                Status INTEGER,
                Remark TEXT,
                FOREIGN KEY (StudentId) REFERENCES Students(Id)
            );

            CREATE TABLE IF NOT EXISTS ClassroomPoints (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId INTEGER,
                Points INTEGER,
                Reason TEXT,
                CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
                FOREIGN KEY (StudentId) REFERENCES Students(Id)
            );

            CREATE TABLE IF NOT EXISTS Notifications (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT,
                Content TEXT,
                ClassId INTEGER,
                CreatedAt TEXT DEFAULT (datetime('now', 'localtime')),
                SentAt TEXT
            );

            CREATE TABLE IF NOT EXISTS NotificationTemplates (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Template TEXT,
                Category TEXT
            );

            CREATE INDEX IF NOT EXISTS idx_students_class ON Students(ClassId);
            CREATE INDEX IF NOT EXISTS idx_scores_student ON Scores(StudentId);
            CREATE INDEX IF NOT EXISTS idx_scores_exam ON Scores(Subject, ExamName);
            CREATE INDEX IF NOT EXISTS idx_attendances_student ON Attendances(StudentId);
            CREATE INDEX IF NOT EXISTS idx_attendances_date ON Attendances(Date);
            CREATE INDEX IF NOT EXISTS idx_classroompoints_student ON ClassroomPoints(StudentId);
        ";

        using var command = connection.CreateCommand();
        command.CommandText = createTablesSql;
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null;
    }
}
