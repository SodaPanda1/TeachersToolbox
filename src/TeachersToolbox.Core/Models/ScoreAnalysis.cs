namespace TeachersToolbox.Core.Models;

public class ScoreChangeAnalysis
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal PreviousScore { get; set; }
    public decimal CurrentScore { get; set; }
    public decimal Change { get; set; }
    public decimal ChangeRate { get; set; }
    public ScoreChangeType ChangeType { get; set; }
}

public enum ScoreChangeType
{
    Improved,
    Declined,
    Stable
}

public class ClassComparison
{
    public string ClassName { get; set; } = string.Empty;
    public decimal Average { get; set; }
    public decimal Max { get; set; }
    public decimal Min { get; set; }
    public decimal PassRate { get; set; }
    public decimal ExcellentRate { get; set; }
}

public class SubjectAnalysis
{
    public string Subject { get; set; } = string.Empty;
    public decimal ClassAverage { get; set; }
    public decimal HighestScore { get; set; }
    public decimal LowestScore { get; set; }
    public List<ScoreDistribution> Distribution { get; set; } = new();
}

public class ScoreDistribution
{
    public string Range { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}
