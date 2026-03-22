using Dapper;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data;

namespace TeachersToolbox.Data.Repositories;

public class AssignmentRepository
{
    private readonly DbContext _context;

    public AssignmentRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<Assignment?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        return await connection.QuerySingleOrDefaultAsync<Assignment>(
            "SELECT * FROM Assignments WHERE Id = @Id", new { Id = id });
    }

    public async Task<List<Assignment>> GetByClassIdAsync(int classId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var result = await connection.QueryAsync<Assignment>(
            "SELECT * FROM Assignments WHERE ClassId = @ClassId ORDER BY DueDate DESC",
            new { ClassId = classId });
        return result.ToList();
    }

    public async Task<Assignment> AddAsync(Assignment assignment)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"INSERT INTO Assignments (ClassId, Title, Description, Subject, DueDate) 
                    VALUES (@ClassId, @Title, @Description, @Subject, @DueDate);
                    SELECT last_insert_rowid();";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, assignment);
        assignment.Id = id;
        return assignment;
    }

    public async Task UpdateAsync(Assignment assignment)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"UPDATE Assignments SET ClassId = @ClassId, Title = @Title,
                    Description = @Description, Subject = @Subject, DueDate = @DueDate
                    WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, assignment);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        await connection.ExecuteAsync("DELETE FROM Assignments WHERE Id = @Id", new { Id = id });
    }

    public async Task<AssignmentRecord> AddRecordAsync(AssignmentRecord record)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"INSERT INTO AssignmentRecords (AssignmentId, StudentId, IsCompleted, Score, Remark, CompletedAt) 
                    VALUES (@AssignmentId, @StudentId, @IsCompleted, @Score, @Remark, @CompletedAt);
                    SELECT last_insert_rowid();";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, record);
        record.Id = id;
        return record;
    }

    public async Task UpdateRecordAsync(AssignmentRecord record)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"UPDATE AssignmentRecords SET AssignmentId = @AssignmentId, 
                    StudentId = @StudentId, IsCompleted = @IsCompleted, 
                    Score = @Score, Remark = @Remark, CompletedAt = @CompletedAt
                    WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, record);
    }

    public async Task<List<AssignmentRecord>> GetRecordsAsync(int assignmentId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var result = await connection.QueryAsync<AssignmentRecord>(
            "SELECT * FROM AssignmentRecords WHERE AssignmentId = @AssignmentId",
            new { AssignmentId = assignmentId });
        return result.ToList();
    }

    public async Task<(int completed, int total, decimal avgScore)> GetStatisticsAsync(int assignmentId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT 
                    SUM(CASE WHEN IsCompleted = 1 THEN 1 ELSE 0 END) as completed,
                    COUNT(*) as total,
                    AVG(CASE WHEN Score > 0 THEN CAST(Score AS REAL) ELSE NULL END) as avgScore
                    FROM AssignmentRecords WHERE AssignmentId = @AssignmentId";
        
        var result = await connection.QuerySingleAsync<(int completed, int total, decimal avgScore)>(
            sql, new { AssignmentId = assignmentId });
        return result;
    }
}
