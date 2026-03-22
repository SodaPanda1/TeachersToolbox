using Dapper;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data;

namespace TeachersToolbox.Data.Repositories;

public class ScoreRepository
{
    private readonly DbContext _context;

    public ScoreRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<Score?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        return await connection.QuerySingleOrDefaultAsync<Score>(
            "SELECT * FROM Scores WHERE Id = @Id", new { Id = id });
    }

    public async Task<List<Score>> GetByStudentIdAsync(int studentId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var result = await connection.QueryAsync<Score>(
            "SELECT * FROM Scores WHERE StudentId = @StudentId ORDER BY ExamDate DESC",
            new { StudentId = studentId });
        return result.ToList();
    }

    public async Task<List<Score>> GetByClassIdAsync(int classId, string? subject = null)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT s.* FROM Scores s 
                    INNER JOIN Students st ON s.StudentId = st.Id 
                    WHERE st.ClassId = @ClassId";
        
        if (!string.IsNullOrEmpty(subject))
            sql += " AND s.Subject = @Subject";
        
        sql += " ORDER BY s.ExamDate DESC, st.SeatNumber";
        
        var result = await connection.QueryAsync<Score>(sql, 
            new { ClassId = classId, Subject = subject });
        return result.ToList();
    }

    public async Task<List<Score>> GetByExamAsync(int classId, string subject, string examName)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT s.* FROM Scores s 
                    INNER JOIN Students st ON s.StudentId = st.Id 
                    WHERE st.ClassId = @ClassId AND s.Subject = @Subject AND s.ExamName = @ExamName
                    ORDER BY s.Value DESC";
        
        var result = await connection.QueryAsync<Score>(sql,
            new { ClassId = classId, Subject = subject, ExamName = examName });
        return result.ToList();
    }

    public async Task<Score> AddAsync(Score score)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"INSERT INTO Scores (StudentId, Subject, Value, MaxValue, 
                    ExamType, ExamName, ExamDate, Remark) 
                    VALUES (@StudentId, @Subject, @Value, @MaxValue, 
                    @ExamType, @ExamName, @ExamDate, @Remark);
                    SELECT last_insert_rowid();";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, score);
        score.Id = id;
        return score;
    }

    public async Task AddBatchAsync(List<Score> scores)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"INSERT INTO Scores (StudentId, Subject, Value, MaxValue, 
                    ExamType, ExamName, ExamDate, Remark) 
                    VALUES (@StudentId, @Subject, @Value, @MaxValue, 
                    @ExamType, @ExamName, @ExamDate, @Remark)";
        
        await connection.ExecuteAsync(sql, scores);
    }

    public async Task UpdateAsync(Score score)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"UPDATE Scores SET StudentId = @StudentId, Subject = @Subject,
                    Value = @Value, MaxValue = @MaxValue, ExamType = @ExamType,
                    ExamName = @ExamName, ExamDate = @ExamDate, Remark = @Remark
                    WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, score);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        await connection.ExecuteAsync("DELETE FROM Scores WHERE Id = @Id", new { Id = id });
    }

    public async Task<decimal> GetAverageByExamAsync(int classId, string subject, string examName)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT AVG(s.Value) FROM Scores s 
                    INNER JOIN Students st ON s.StudentId = st.Id 
                    WHERE st.ClassId = @ClassId AND s.Subject = @Subject AND s.ExamName = @ExamName";
        
        var result = await connection.ExecuteScalarAsync<decimal?>(sql,
            new { ClassId = classId, Subject = subject, ExamName = examName });
        return result ?? 0;
    }

    public async Task<List<string>> GetExamNamesAsync(int classId, string subject)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT DISTINCT s.ExamName FROM Scores s 
                    INNER JOIN Students st ON s.StudentId = st.Id 
                    WHERE st.ClassId = @ClassId AND s.Subject = @Subject
                    ORDER BY s.ExamDate";
        
        var result = await connection.QueryAsync<string>(sql,
            new { ClassId = classId, Subject = subject });
        return result.ToList();
    }

    public async Task<List<string>> GetSubjectsAsync(int classId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT DISTINCT s.Subject FROM Scores s 
                    INNER JOIN Students st ON s.StudentId = st.Id 
                    WHERE st.ClassId = @ClassId
                    ORDER BY s.Subject";
        
        var result = await connection.QueryAsync<string>(sql,
            new { ClassId = classId });
        return result.ToList();
    }
}
