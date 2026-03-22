namespace TeachersToolbox.Core.Helpers;

public static class StatisticsHelper
{
    public static decimal CalculateAverage(IEnumerable<decimal> values)
    {
        var list = values.ToList();
        return list.Count == 0 ? 0 : list.Average();
    }

    public static decimal CalculateMedian(IEnumerable<decimal> values)
    {
        var sorted = values.OrderBy(x => x).ToList();
        if (sorted.Count == 0) return 0;
        if (sorted.Count % 2 == 0)
            return (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2;
        return sorted[sorted.Count / 2];
    }

    public static decimal CalculateStandardDeviation(IEnumerable<decimal> values)
    {
        var list = values.ToList();
        if (list.Count == 0) return 0;
        var avg = list.Average();
        var sumSquares = list.Sum(x => (x - avg) * (x - avg));
        return (decimal)Math.Sqrt((double)(sumSquares / list.Count));
    }

    public static (int excellent, int good, int pass, int fail) GetGradeDistribution(
        IEnumerable<decimal> scores, 
        decimal excellentLine = 90, 
        decimal goodLine = 80, 
        decimal passLine = 60)
    {
        var list = scores.ToList();
        return (
            list.Count(s => s >= excellentLine),
            list.Count(s => s >= goodLine && s < excellentLine),
            list.Count(s => s >= passLine && s < goodLine),
            list.Count(s => s < passLine)
        );
    }

    public static Dictionary<decimal, int> GetScoreDistribution(IEnumerable<decimal> scores, decimal interval = 10)
    {
        var distribution = new Dictionary<decimal, int>();
        var list = scores.ToList();
        
        for (decimal i = 0; i <= 100; i += interval)
        {
            distribution[i] = list.Count(s => s >= i && s < i + interval);
        }
        
        return distribution;
    }

    public static decimal CalculateChangeRate(decimal current, decimal previous)
    {
        if (previous == 0) return 0;
        return (current - previous) / previous * 100;
    }

    public static List<int> CalculateRanking(List<decimal> scores, bool descending = true)
    {
        var indexed = scores.Select((score, index) => new { score, index }).ToList();
        var sorted = descending 
            ? indexed.OrderByDescending(x => x.score).ToList()
            : indexed.OrderBy(x => x.score).ToList();
        
        var ranks = new int[scores.Count];
        for (int i = 0; i < sorted.Count; i++)
        {
            ranks[sorted[i].index] = i + 1;
        }
        
        return ranks.ToList();
    }
}
