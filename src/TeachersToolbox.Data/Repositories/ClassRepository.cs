using Dapper;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data;

namespace TeachersToolbox.Data.Repositories;

public class ClassRepository
{
    private readonly DbContext _context;

    public ClassRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<Class?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        return await connection.QuerySingleOrDefaultAsync<Class>(
            "SELECT * FROM Classes WHERE Id = @Id", new { Id = id });
    }

    public async Task<List<Class>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var result = await connection.QueryAsync<Class>("SELECT * FROM Classes ORDER BY Name");
        return result.ToList();
    }

    public async Task<Class> AddAsync(Class classEntity)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"INSERT INTO Classes (Name, Grade, Year, Teacher) 
                    VALUES (@Name, @Grade, @Year, @Teacher);
                    SELECT last_insert_rowid();";
        
        var id = await connection.ExecuteScalarAsync<int>(sql, classEntity);
        classEntity.Id = id;
        return classEntity;
    }

    public async Task UpdateAsync(Class classEntity)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        var sql = @"UPDATE Classes SET Name = @Name, Grade = @Grade, 
                    Year = @Year, Teacher = @Teacher WHERE Id = @Id";
        
        await connection.ExecuteAsync(sql, classEntity);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        await ((Microsoft.Data.Sqlite.SqliteConnection)connection).OpenAsync();
        
        await connection.ExecuteAsync("DELETE FROM Classes WHERE Id = @Id", new { Id = id });
    }
}
