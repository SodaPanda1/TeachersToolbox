using Dapper;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data;

namespace TeachersToolbox.Data.Repositories;

public class ClassroomPointRepository
{
    private readonly DbContext _context;

    public ClassroomPointRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<ClassroomPoint> AddAsync(ClassroomPoint point)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"INSERT INTO ClassroomPoints (StudentId, Points, Reason) 
                    VALUES (@StudentId, @Points, @Reason);
                    SELECT last_insert_rowid();";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, point);
        point.Id = id;
        return point;
    }

    public async Task<int> GetTotalPointsAsync(int studentId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT COALESCE(SUM(Points), 0) FROM ClassroomPoints 
                    WHERE StudentId = @StudentId";
        
        return await connection.ExecuteScalarAsync<int>(sql, new { StudentId = studentId });
    }

    public async Task<Dictionary<int, int>> GetClassPointsSummaryAsync(int classId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT s.Id, COALESCE(SUM(cp.Points), 0) as TotalPoints
                    FROM Students s
                    LEFT JOIN ClassroomPoints cp ON s.Id = cp.StudentId
                    WHERE s.ClassId = @ClassId
                    GROUP BY s.Id";
        
        var result = await connection.QueryAsync<(int Id, int TotalPoints)>(
            sql, new { ClassId = classId });
        
        return result.ToDictionary(x => x.Id, x => x.TotalPoints);
    }

    public async Task<List<ClassroomPoint>> GetHistoryAsync(int studentId, int limit = 50)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT * FROM ClassroomPoints 
                    WHERE StudentId = @StudentId 
                    ORDER BY CreatedAt DESC LIMIT @Limit";
        
        var result = await connection.QueryAsync<ClassroomPoint>(
            sql, new { StudentId = studentId, Limit = limit });
        return result.ToList();
    }

    public async Task ResetPointsAsync(int classId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"DELETE FROM ClassroomPoints 
                    WHERE StudentId IN (SELECT Id FROM Students WHERE ClassId = @ClassId)";
        
        await connection.ExecuteAsync(sql, new { ClassId = classId });
    }
}
