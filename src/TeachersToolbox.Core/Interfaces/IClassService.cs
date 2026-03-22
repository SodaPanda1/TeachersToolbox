using TeachersToolbox.Core.Models;

namespace TeachersToolbox.Core.Interfaces;

public interface IClassService
{
    Task<Class?> GetByIdAsync(int id);
    Task<List<Class>> GetAllAsync();
    Task<Class> AddAsync(Class classEntity);
    Task UpdateAsync(Class classEntity);
    Task DeleteAsync(int id);
    Task<List<Student>> GetStudentsAsync(int classId);
}
