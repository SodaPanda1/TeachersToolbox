namespace TeachersToolbox.Core.Models;

public class Class
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Teacher { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<Student> Students { get; set; } = new();
}
