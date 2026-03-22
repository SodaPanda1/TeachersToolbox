namespace TeachersToolbox.Core.Models;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int? ClassId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? SentAt { get; set; }
}

public class NotificationTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Template { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
