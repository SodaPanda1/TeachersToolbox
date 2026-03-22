using Dapper;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data;

namespace TeachersToolbox.Data.Repositories;

public class CourseRepository
{
    private readonly DbContext _context;

    public CourseRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        return await connection.QuerySingleOrDefaultAsync<Course>(
            "SELECT * FROM Courses WHERE Id = @Id", new { Id = id });
    }

    public async Task<List<Course>> GetByClassIdAsync(int classId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var result = await connection.QueryAsync<Course>(
            "SELECT * FROM Courses WHERE ClassId = @ClassId ORDER BY Day, Period",
            new { ClassId = classId });
        return result.ToList();
    }

    public async Task<CourseSchedule?> GetScheduleAsync(int classId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var schedule = await connection.QuerySingleOrDefaultAsync<CourseSchedule>(
            "SELECT * FROM CourseSchedules WHERE ClassId = @ClassId",
            new { ClassId = classId });
        
        if (schedule != null)
        {
            schedule.Courses = (await GetByClassIdAsync(classId)).ToList();
        }
        
        return schedule;
    }

    public async Task<Course> AddAsync(Course course)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"INSERT INTO Courses (ClassId, Subject, Teacher, Day, Period, Room) 
                    VALUES (@ClassId, @Subject, @Teacher, @Day, @Period, @Room);
                    SELECT last_insert_rowid();";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, course);
        course.Id = id;
        return course;
    }

    public async Task UpdateAsync(Course course)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"UPDATE Courses SET ClassId = @ClassId, Subject = @Subject,
                    Teacher = @Teacher, Day = @Day, Period = @Period, Room = @Room
                    WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, course);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        await connection.ExecuteAsync("DELETE FROM Courses WHERE Id = @Id", new { Id = id });
    }

    public async Task<CourseSchedule> SaveScheduleAsync(CourseSchedule schedule)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        if (schedule.Id == 0)
        {
            var sql = @"INSERT INTO CourseSchedules (ClassId, Name) 
                        VALUES (@ClassId, @Name); SELECT last_insert_rowid();";
            schedule.Id = await connection.ExecuteScalarAsync<int>(sql, schedule);
        }
        else
        {
            await connection.ExecuteAsync(
                "UPDATE CourseSchedules SET Name = @Name WHERE Id = @Id", schedule);
        }
        
        return schedule;
    }
}
