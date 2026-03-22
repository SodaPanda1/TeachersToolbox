namespace TeachersToolbox.Core.Models;

public class Assignment
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<AssignmentRecord> Records { get; set; } = new();
}

public class AssignmentRecord
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public bool IsCompleted { get; set; }
    public int Score { get; set; }
    public string Remark { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
}
