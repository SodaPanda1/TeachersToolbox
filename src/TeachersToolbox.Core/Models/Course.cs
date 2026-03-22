namespace TeachersToolbox.Core.Models;

public class Course
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Teacher { get; set; } = string.Empty;
    public DayOfWeek Day { get; set; }
    public int Period { get; set; }
    public string Room { get; set; } = string.Empty;
}

public class CourseSchedule
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Course> Courses { get; set; } = new();
}
