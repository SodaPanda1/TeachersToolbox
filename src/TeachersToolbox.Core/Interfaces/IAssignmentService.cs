using TeachersToolbox.Core.Models;

namespace TeachersToolbox.Core.Interfaces;

public interface IAssignmentService
{
    Task<Assignment?> GetByIdAsync(int id);
    Task<List<Assignment>> GetByClassIdAsync(int classId);
    Task<Assignment> AddAsync(Assignment assignment);
    Task UpdateAsync(Assignment assignment);
    Task DeleteAsync(int id);
    Task<AssignmentRecord> AddRecordAsync(AssignmentRecord record);
    Task UpdateRecordAsync(AssignmentRecord record);
    Task<List<AssignmentRecord>> GetRecordsAsync(int assignmentId);
    Task<AssignmentStatistics> GetStatisticsAsync(int assignmentId);
}

public class AssignmentStatistics
{
    public int TotalCount { get; set; }
    public int CompletedCount { get; set; }
    public int PendingCount { get; set; }
    public decimal CompletionRate => TotalCount > 0 ? (decimal)CompletedCount / TotalCount * 100 : 0;
    public decimal AverageScore { get; set; }
}
