using TeachersToolbox.Core.Models;

namespace TeachersToolbox.Core.Interfaces;

public interface IScoreService
{
    Task<Score?> GetByIdAsync(int id);
    Task<List<Score>> GetByStudentIdAsync(int studentId);
    Task<List<Score>> GetByClassIdAsync(int classId, string? subject = null);
    Task<Score> AddAsync(Score score);
    Task AddBatchAsync(List<Score> scores);
    Task UpdateAsync(Score score);
    Task DeleteAsync(int id);
    Task<ScoreStatistics> GetStatisticsAsync(int classId, string subject, string examName);
    Task<List<ScoreTrend>> GetTrendsAsync(int studentId, string subject);
    Task<List<ScoreRank>> GetRankingAsync(int classId, string subject, string examName);
}

public class ScoreStatistics
{
    public decimal Average { get; set; }
    public decimal Max { get; set; }
    public decimal Min { get; set; }
    public decimal Median { get; set; }
    public decimal StandardDeviation { get; set; }
    public int ExcellentCount { get; set; }
    public int GoodCount { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public int TotalCount { get; set; }
    public decimal ExcellentRate => TotalCount > 0 ? (decimal)ExcellentCount / TotalCount * 100 : 0;
    public decimal PassRate => TotalCount > 0 ? (decimal)(ExcellentCount + GoodCount + PassCount) / TotalCount * 100 : 0;
}

public class ScoreTrend
{
    public string ExamName { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public decimal Score { get; set; }
    public decimal ClassAverage { get; set; }
}

public class ScoreRank
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public int Rank { get; set; }
    public decimal ChangeFromLast { get; set; }
}
