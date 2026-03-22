using TeachersToolbox.Core.Models;

namespace TeachersToolbox.Core.Services;

public class RandomPickerService
{
    private readonly Random _random = new();

    public Student? PickRandom(List<Student> students, List<int>? excludeIds = null)
    {
        var available = students.Where(s => excludeIds == null || !excludeIds.Contains(s.Id)).ToList();
        if (available.Count == 0) return null;
        return available[_random.Next(available.Count)];
    }

    public List<Student> PickMultiple(List<Student> students, int count, bool allowDuplicate = false)
    {
        if (count >= students.Count) return students.ToList();

        if (allowDuplicate)
        {
            return Enumerable.Range(0, count)
                .Select(_ => students[_random.Next(students.Count)])
                .ToList();
        }

        var available = students.ToList();
        var result = new List<Student>();
        
        for (int i = 0; i < count && available.Count > 0; i++)
        {
            var index = _random.Next(available.Count);
            result.Add(available[index]);
            available.RemoveAt(index);
        }
        
        return result;
    }

    public List<List<Student>> DivideIntoGroups(List<Student> students, int groupCount)
    {
        if (groupCount <= 0 || students.Count == 0)
            return new List<List<Student>>();

        var shuffled = students.OrderBy(_ => _random.Next()).ToList();
        var groups = Enumerable.Range(0, groupCount).Select(_ => new List<Student>()).ToList();

        for (int i = 0; i < shuffled.Count; i++)
        {
            groups[i % groupCount].Add(shuffled[i]);
        }

        return groups;
    }

    public List<List<Student>> DivideIntoGroupsWithSize(List<Student> students, int groupSize)
    {
        if (groupSize <= 0 || students.Count == 0)
            return new List<List<Student>>();

        var shuffled = students.OrderBy(_ => _random.Next()).ToList();
        var groups = new List<List<Student>>();

        for (int i = 0; i < shuffled.Count; i += groupSize)
        {
            groups.Add(shuffled.Skip(i).Take(groupSize).ToList());
        }

        return groups;
    }

    public List<int> ShuffleSeatNumbers(int count)
    {
        return Enumerable.Range(1, count).OrderBy(_ => _random.Next()).ToList();
    }

    public string PickRandomFromList(List<string> items)
    {
        if (items.Count == 0) return string.Empty;
        return items[_random.Next(items.Count)];
    }

    public int GenerateRandomNumber(int min, int max)
    {
        return _random.Next(min, max + 1);
    }

    public List<int> GenerateRandomNumbers(int min, int max, int count)
    {
        var numbers = new List<int>();
        var available = Enumerable.Range(min, max - min + 1).ToList();

        for (int i = 0; i < count && available.Count > 0; i++)
        {
            var index = _random.Next(available.Count);
            numbers.Add(available[index]);
            available.RemoveAt(index);
        }

        return numbers;
    }
}
