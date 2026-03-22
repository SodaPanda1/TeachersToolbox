using Dapper;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data;

namespace TeachersToolbox.Data.Repositories;

public class AttendanceRepository
{
    private readonly DbContext _context;

    public AttendanceRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<List<Attendance>> GetByDateAsync(DateTime date, int? classId = null)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var dateStr = date.ToString("yyyy-MM-dd");
        
        if (classId.HasValue)
        {
            var sql = @"SELECT a.* FROM Attendances a 
                        INNER JOIN Students s ON a.StudentId = s.Id 
                        WHERE a.Date = @Date AND s.ClassId = @ClassId";
            return (await connection.QueryAsync<Attendance>(sql, 
                new { Date = dateStr, ClassId = classId.Value })).ToList();
        }
        
        return (await connection.QueryAsync<Attendance>(
            "SELECT * FROM Attendances WHERE Date = @Date", new { Date = dateStr })).ToList();
    }

    public async Task<List<Attendance>> GetByStudentIdAsync(int studentId, 
        DateTime? startDate = null, DateTime? endDate = null)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = "SELECT * FROM Attendances WHERE StudentId = @StudentId";
        var parameters = new DynamicParameters();
        parameters.Add("StudentId", studentId);
        
        if (startDate.HasValue)
        {
            sql += " AND Date >= @StartDate";
            parameters.Add("StartDate", startDate.Value.ToString("yyyy-MM-dd"));
        }
        if (endDate.HasValue)
        {
            sql += " AND Date <= @EndDate";
            parameters.Add("EndDate", endDate.Value.ToString("yyyy-MM-dd"));
        }
        
        sql += " ORDER BY Date DESC";
        
        return (await connection.QueryAsync<Attendance>(sql, parameters)).ToList();
    }

    public async Task<Attendance> AddAsync(Attendance attendance)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"INSERT INTO Attendances (StudentId, Date, Status, Remark) 
                    VALUES (@StudentId, @Date, @Status, @Remark);
                    SELECT last_insert_rowid();";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, attendance);
        attendance.Id = id;
        return attendance;
    }

    public async Task AddBatchAsync(List<Attendance> attendances)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"INSERT INTO Attendances (StudentId, Date, Status, Remark) 
                    VALUES (@StudentId, @Date, @Status, @Remark)";
        
        await connection.ExecuteAsync(sql, attendances);
    }

    public async Task UpdateAsync(Attendance attendance)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"UPDATE Attendances SET StudentId = @StudentId, Date = @Date,
                    Status = @Status, Remark = @Remark WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, attendance);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        await connection.ExecuteAsync("DELETE FROM Attendances WHERE Id = @Id", new { Id = id });
    }

    public async Task<Dictionary<int, (int present, int absent, int late)>> GetStatisticsByClassAsync(
        int classId, DateTime startDate, DateTime endDate)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT s.Id as StudentId,
                    SUM(CASE WHEN a.Status = 0 THEN 1 ELSE 0 END) as present,
                    SUM(CASE WHEN a.Status = 1 THEN 1 ELSE 0 END) as absent,
                    SUM(CASE WHEN a.Status = 2 THEN 1 ELSE 0 END) as late
                    FROM Students s
                    LEFT JOIN Attendances a ON s.Id = a.StudentId 
                        AND a.Date BETWEEN @StartDate AND @EndDate
                    WHERE s.ClassId = @ClassId
                    GROUP BY s.Id";
        
        var result = await connection.QueryAsync<(int StudentId, int present, int absent, int late)>(
            sql, new { ClassId = classId, StartDate = startDate.ToString("yyyy-MM-dd"), 
                      EndDate = endDate.ToString("yyyy-MM-dd") });
        
        return result.ToDictionary(x => x.StudentId, x => (x.present, x.absent, x.late));
    }
}
