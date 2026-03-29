using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data.Repositories;

namespace TeachersToolbox.App.Views;

public sealed partial class RollCallPage : Page
{
    private readonly ClassRepository _classRepository;
    private readonly StudentRepository _studentRepository;
    private List<Class> _classes = new();
    private List<Student> _students = new();
    private List<int> _calledStudentIds = new();
    private System.Threading.Timer? _rollTimer;
    private Random _random = new();
    private bool _isRolling = false;
    private Student? _currentStudent;

    public RollCallPage()
    {
        this.InitializeComponent();
        _classRepository = App.Services.GetRequiredService<ClassRepository>();
        _studentRepository = App.Services.GetRequiredService<StudentRepository>();
        this.Loaded += Page_Loaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadClassesAsync();
    }

    private async Task LoadClassesAsync()
    {
        try
        {
            _classes = await _classRepository.GetAllAsync();
            ClassComboBox.Items.Clear();
            
            // 添加"全部"选项
            ClassComboBox.Items.Add(new ComboBoxItem { Content = "全部", Tag = -1 });
            
            foreach (var cls in _classes)
            {
                ClassComboBox.Items.Add(new ComboBoxItem { Content = cls.Name, Tag = cls.Id });
            }
            
            if (ClassComboBox.Items.Count > 0)
            {
                ClassComboBox.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载班级失败: {ex.Message}");
        }
    }

    private async void ClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ClassComboBox.SelectedItem is ComboBoxItem item)
        {
            var classId = (int)item.Tag;
            await LoadStudentsAsync(classId);
        }
    }

    private async Task LoadStudentsAsync(int classId)
    {
        try
        {
            _students.Clear();
            _calledStudentIds.Clear();
            
            if (classId == -1)
            {
                // 加载全部学生
                foreach (var cls in _classes)
                {
                    var students = await _studentRepository.GetByClassIdAsync(cls.Id);
                    _students.AddRange(students);
                }
            }
            else
            {
                // 加载指定班级的学生
                _students = await _studentRepository.GetByClassIdAsync(classId);
            }
            
            if (_students.Count > 0)
            {
                ClassInfoText.Text = $"已加载 {_students.Count} 名学生";
                StudentNameText.Text = "准备好了吗？";
            }
            else
            {
                ClassInfoText.Text = "该班级暂无学生";
                StudentNameText.Text = "请先导入学生";
            }
            
            UpdateHistory();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载学生失败: {ex.Message}");
            ClassInfoText.Text = "加载失败";
        }
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
