namespace TeachersToolbox.Core.Models;

public class ClassroomPoint
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int Points { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
