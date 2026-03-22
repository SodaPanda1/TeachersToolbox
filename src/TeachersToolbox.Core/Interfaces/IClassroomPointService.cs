using TeachersToolbox.Core.Models;

namespace TeachersToolbox.Core.Interfaces;

public interface IClassroomPointService
{
    Task<ClassroomPoint> AddPointsAsync(int studentId, int points, string reason);
    Task<int> GetTotalPointsAsync(int studentId);
    Task<Dictionary<int, int>> GetClassPointsSummaryAsync(int classId);
    Task<List<ClassroomPoint>> GetHistoryAsync(int studentId);
    Task ResetPointsAsync(int classId);
}
