using TeachersToolbox.Core.Models;

namespace TeachersToolbox.Core.Interfaces;

public interface IAttendanceService
{
    Task<List<Attendance>> GetByDateAsync(DateTime date, int? classId = null);
    Task<List<Attendance>> GetByStudentIdAsync(int studentId, DateTime? startDate = null, DateTime? endDate = null);
    Task<Attendance> AddAsync(Attendance attendance);
    Task AddBatchAsync(List<Attendance> attendances);
    Task UpdateAsync(Attendance attendance);
    Task<AttendanceStatistics> GetStatisticsAsync(int classId, DateTime startDate, DateTime endDate);
}

public class AttendanceStatistics
{
    public int TotalDays { get; set; }
    public Dictionary<int, StudentAttendanceSummary> StudentSummaries { get; set; } = new();
}

public class StudentAttendanceSummary
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LateDays { get; set; }
    public int ExcusedDays { get; set; }
    public decimal AttendanceRate => TotalDays > 0 ? (decimal)PresentDays / TotalDays * 100 : 0;
    public int TotalDays { get; set; }
}
