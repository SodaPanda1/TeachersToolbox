using TeachersToolbox.Core.Models;

namespace TeachersToolbox.Core.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportScoresToExcelAsync(int classId, string subject, string examName);
    Task<byte[]> ExportStudentListToExcelAsync(int classId);
    Task<byte[]> ExportAttendanceToExcelAsync(int classId, DateTime startDate, DateTime endDate);
    Task<string> ExportScoresToCsvAsync(int classId, string subject, string examName);
    Task<byte[]> GenerateReportCardAsync(int studentId);
}
