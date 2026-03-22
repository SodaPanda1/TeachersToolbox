namespace TeachersToolbox.Core.Models;

public class Score
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal MaxValue { get; set; } = 100;
    public string ExamType { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public string Remark { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
