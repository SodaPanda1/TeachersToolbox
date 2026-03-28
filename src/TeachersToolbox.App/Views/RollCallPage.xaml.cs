using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TeachersToolbox.Core.Models;

namespace TeachersToolbox.App.Views;

public sealed partial class RollCallPage : Page
{
    private List<Student> _students = new();
    private List<int> _calledStudentIds = new();
    private System.Threading.Timer? _rollTimer;
    private Random _random = new();
    private bool _isRolling = false;
    private Student? _currentStudent;

    public RollCallPage()
    {
        this.InitializeComponent();
    }

    private void ClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadStudents();
    }

    private void LoadStudents()
    {
        // TODO: 从数据库加载学生
        // 移除示例数据，显示空列表提示
        _students.Clear();
        ClassInfoText.Text = "请先在学生管理中导入学生名单";
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (_students.Count == 0)
        {
            StudentNameText.Text = "暂无学生数据";
            return;
        }

        _isRolling = true;
        StartButton.IsEnabled = false;
        StopButton.IsEnabled = true;

        if (AnimationToggle.IsOn)
        {
            _rollTimer = new System.Threading.Timer(RollName, null, 0, 50);
        }
        else
        {
            PickRandomStudent();
            _isRolling = false;
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }
    }

    private void RollName(object? state)
    {
        if (!_isRolling) return;

        DispatcherQueue.TryEnqueue(() =>
        {
            var available = RepeatToggle.IsOn
                ? _students
                : _students.Where(s => !_calledStudentIds.Contains(s.Id)).ToList();

            if (available.Count == 0)
            {
                StopRolling();
                StudentNameText.Text = "所有人都已点过！";
                return;
            }

            var student = available[_random.Next(available.Count)];
            StudentNameText.Text = student.Name;
        });
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        StopRolling();
        PickRandomStudent();
    }

    private void StopRolling()
    {
        _isRolling = false;
        _rollTimer?.Dispose();
        _rollTimer = null;
        StartButton.IsEnabled = true;
        StopButton.IsEnabled = false;
    }

    private void PickRandomStudent()
    {
        var available = RepeatToggle.IsOn
            ? _students
            : _students.Where(s => !_calledStudentIds.Contains(s.Id)).ToList();

        if (available.Count == 0)
        {
            StudentNameText.Text = "所有人都已点过！";
            return;
        }

        _currentStudent = available[_random.Next(available.Count)];
        StudentNameText.Text = _currentStudent.Name;

        if (!RepeatToggle.IsOn)
        {
            _calledStudentIds.Add(_currentStudent.Id);
        }

        UpdateHistory();
    }

    private void UpdateHistory()
    {
        var calledNames = _calledStudentIds
            .Select(id => _students.FirstOrDefault(s => s.Id == id)?.Name)
            .Where(name => name != null);

        HistoryText.Text = string.Join(" → ", calledNames);
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        StopRolling();
        _calledStudentIds.Clear();
        StudentNameText.Text = "准备好了吗？";
        HistoryText.Text = "暂无记录";
    }
}
