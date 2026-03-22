using TeachersToolbox.Core.Helpers;
using TeachersToolbox.Core.Services;
using TeachersToolbox.Core.Models;

namespace TeachersToolbox.Core.Tests;

public class StatisticsHelperTests
{
    [Fact]
    public void CalculateAverage_ReturnsCorrectValue()
    {
        var scores = new List<decimal> { 80, 90, 70, 85 };
        var average = StatisticsHelper.CalculateAverage(scores);
        Assert.Equal(81.25m, average);
    }

    [Fact]
    public void CalculateAverage_EmptyList_ReturnsZero()
    {
        var scores = new List<decimal>();
        var average = StatisticsHelper.CalculateAverage(scores);
        Assert.Equal(0, average);
    }

    [Fact]
    public void CalculateMedian_OddCount_ReturnsMiddleValue()
    {
        var scores = new List<decimal> { 70, 80, 90 };
        var median = StatisticsHelper.CalculateMedian(scores);
        Assert.Equal(80, median);
    }

    [Fact]
    public void CalculateMedian_EvenCount_ReturnsAverageOfMiddle()
    {
        var scores = new List<decimal> { 70, 80, 85, 90 };
        var median = StatisticsHelper.CalculateMedian(scores);
        Assert.Equal(82.5m, median);
    }

    [Fact]
    public void CalculateStandardDeviation_ReturnsCorrectValue()
    {
        var scores = new List<decimal> { 80, 90, 70, 85, 75 };
        var stdDev = StatisticsHelper.CalculateStandardDeviation(scores);
        Assert.True(stdDev > 0);
    }

    [Fact]
    public void GetGradeDistribution_ReturnsCorrectCounts()
    {
        var scores = new List<decimal> { 95, 85, 75, 55 };
        var (excellent, good, pass, fail) = StatisticsHelper.GetGradeDistribution(scores);
        
        Assert.Equal(1, excellent);
        Assert.Equal(1, good);
        Assert.Equal(1, pass);
        Assert.Equal(1, fail);
    }
}

public class RandomPickerServiceTests
{
    private readonly RandomPickerService _service = new();

    [Fact]
    public void PickRandom_ReturnsStudentFromList()
    {
        var students = new List<Student>
        {
            new Student { Id = 1, Name = "张三" },
            new Student { Id = 2, Name = "李四" },
            new Student { Id = 3, Name = "王五" }
        };

        var result = _service.PickRandom(students);
        
        Assert.NotNull(result);
        Assert.Contains(result, students);
    }

    [Fact]
    public void PickRandom_ExcludesSpecificStudents()
    {
        var students = new List<Student>
        {
            new Student { Id = 1, Name = "张三" },
            new Student { Id = 2, Name = "李四" },
            new Student { Id = 3, Name = "王五" }
        };

        var excludeIds = new List<int> { 1, 2 };
        var result = _service.PickRandom(students, excludeIds);
        
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
    }

    [Fact]
    public void PickRandom_ReturnsNull_WhenAllExcluded()
    {
        var students = new List<Student>
        {
            new Student { Id = 1, Name = "张三" },
            new Student { Id = 2, Name = "李四" }
        };

        var excludeIds = new List<int> { 1, 2 };
        var result = _service.PickRandom(students, excludeIds);
        
        Assert.Null(result);
    }

    [Fact]
    public void DivideIntoGroups_CreatesCorrectGroupCount()
    {
        var students = new List<Student>
        {
            new Student { Id = 1, Name = "学生1" },
            new Student { Id = 2, Name = "学生2" },
            new Student { Id = 3, Name = "学生3" },
            new Student { Id = 4, Name = "学生4" },
            new Student { Id = 5, Name = "学生5" },
            new Student { Id = 6, Name = "学生6" }
        };

        var groups = _service.DivideIntoGroups(students, 3);
        
        Assert.Equal(3, groups.Count);
        Assert.Equal(2, groups[0].Count);
        Assert.Equal(2, groups[1].Count);
        Assert.Equal(2, groups[2].Count);
    }

    [Fact]
    public void PickMultiple_ReturnsCorrectCount()
    {
        var students = new List<Student>
        {
            new Student { Id = 1, Name = "学生1" },
            new Student { Id = 2, Name = "学生2" },
            new Student { Id = 3, Name = "学生3" },
            new Student { Id = 4, Name = "学生4" },
            new Student { Id = 5, Name = "学生5" }
        };

        var result = _service.PickMultiple(students, 3, false);
        
        Assert.Equal(3, result.Count);
        Assert.Equal(3, result.Distinct().Count());
    }

    [Fact]
    public void GenerateRandomNumbers_ReturnsCorrectCount()
    {
        var numbers = _service.GenerateRandomNumbers(1, 10, 5);
        
        Assert.Equal(5, numbers.Count);
        Assert.True(numbers.All(n => n >= 1 && n <= 10));
    }
}
