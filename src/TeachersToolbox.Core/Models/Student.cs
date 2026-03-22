namespace TeachersToolbox.Core.Models;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public int SeatNumber { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<Score> Scores { get; set; } = new();
}
