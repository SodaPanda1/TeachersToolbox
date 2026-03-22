namespace TeachersToolbox.Core.Models;

public class Attendance
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public DateTime Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Remark { get; set; } = string.Empty;
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    Excused,
    EarlyLeave
}
