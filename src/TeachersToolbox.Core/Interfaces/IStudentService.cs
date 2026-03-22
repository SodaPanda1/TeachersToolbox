using TeachersToolbox.Core.Models;

namespace TeachersToolbox.Core.Interfaces;

public interface IStudentService
{
    Task<Student?> GetByIdAsync(int id);
    Task<List<Student>> GetAllAsync();
    Task<List<Student>> GetByClassIdAsync(int classId);
    Task<Student> AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(int id);
    Task<Student?> GetRandomStudentAsync(int classId, List<int>? excludeIds = null);
    Task<List<List<Student>>> GetRandomGroupsAsync(int classId, int groupCount);
    Task<List<Student>> SearchAsync(string keyword);
}
