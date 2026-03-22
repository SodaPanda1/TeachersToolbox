using TeachersToolbox.Core.Models;

namespace TeachersToolbox.Core.Interfaces;

public interface ICourseService
{
    Task<Course?> GetByIdAsync(int id);
    Task<List<Course>> GetByClassIdAsync(int classId);
    Task<CourseSchedule?> GetScheduleAsync(int classId);
    Task<Course> AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(int id);
    Task<CourseSchedule> SaveScheduleAsync(CourseSchedule schedule);
}
