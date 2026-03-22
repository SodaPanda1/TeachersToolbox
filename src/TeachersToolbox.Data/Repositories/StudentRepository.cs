using Dapper;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data;

namespace TeachersToolbox.Data.Repositories;

public class StudentRepository
{
    private readonly DbContext _context;

    public StudentRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        return await connection.QuerySingleOrDefaultAsync<Student>(
            "SELECT * FROM Students WHERE Id = @Id", new { Id = id });
    }

    public async Task<List<Student>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var result = await connection.QueryAsync<Student>("SELECT * FROM Students ORDER BY Name");
        return result.ToList();
    }

    public async Task<List<Student>> GetByClassIdAsync(int classId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var result = await connection.QueryAsync<Student>(
            "SELECT * FROM Students WHERE ClassId = @ClassId ORDER BY SeatNumber, Name",
            new { ClassId = classId });
        return result.ToList();
    }

    public async Task<Student> AddAsync(Student student)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"INSERT INTO Students (Name, StudentNumber, ClassId, SeatNumber, 
                    Gender, Phone, ParentPhone, Note) 
                    VALUES (@Name, @StudentNumber, @ClassId, @SeatNumber, 
                    @Gender, @Phone, @ParentPhone, @Note);
                    SELECT last_insert_rowid();";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, student);
        student.Id = id;
        return student;
    }

    public async Task UpdateAsync(Student student)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"UPDATE Students SET Name = @Name, StudentNumber = @StudentNumber,
                    ClassId = @ClassId, SeatNumber = @SeatNumber, Gender = @Gender,
                    Phone = @Phone, ParentPhone = @ParentPhone, Note = @Note
                    WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, student);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        await connection.ExecuteAsync("DELETE FROM Students WHERE Id = @Id", new { Id = id });
    }

    public async Task<List<Student>> SearchAsync(string keyword)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"SELECT * FROM Students 
                    WHERE Name LIKE @Keyword OR StudentNumber LIKE @Keyword 
                    ORDER BY Name";
        
        var result = await connection.QueryAsync<Student>(sql, 
            new { Keyword = $"%{keyword}%" });
        return result.ToList();
    }

    public async Task<int> GetCountByClassIdAsync(int classId)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM Students WHERE ClassId = @ClassId",
            new { ClassId = classId });
    }
}
