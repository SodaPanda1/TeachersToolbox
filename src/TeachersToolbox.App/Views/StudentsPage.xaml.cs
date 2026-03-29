using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TeachersToolbox.App.Services;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data.Repositories;

namespace TeachersToolbox.App.Views;

public sealed partial class StudentsPage : Page
{
    private readonly ClassRepository _classRepository;
    private readonly StudentRepository _studentRepository;
    private List<Class> _classes = new();
    private List<StudentWithClass> _students = new();

    public StudentsPage()
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
            
            ClassComboBox.SelectedIndex = 0;
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
            
            if (classId == -1)
            {
                // 加载全部学生
                foreach (var cls in _classes)
                {
                    var students = await _studentRepository.GetByClassIdAsync(cls.Id);
                    foreach (var student in students)
                    {
                        _students.Add(new StudentWithClass
                        {
                            Student = student,
                            ClassName = cls.Name
                        });
                    }
                }
            }
            else
            {
                // 加载指定班级的学生
                var classInfo = _classes.FirstOrDefault(c => c.Id == classId);
                var students = await _studentRepository.GetByClassIdAsync(classId);
                foreach (var student in students)
                {
                    _students.Add(new StudentWithClass
                    {
                        Student = student,
                        ClassName = classInfo?.Name ?? "未知"
                    });
                }
            }
            
            StudentListView.ItemsSource = _students;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载学生失败: {ex.Message}");
        }
    }

    private async void AddClassButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new AddClassDialog();
        dialog.XamlRoot = this.XamlRoot;

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(dialog.ClassName))
        {
            try
            {
                var newClass = new Class { Name = dialog.ClassName };
                await _classRepository.AddAsync(newClass);
                await LoadClassesAsync();
            }
            catch (Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "添加失败",
                    Content = ex.Message,
                    CloseButtonText = "确定",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
    }

    private async void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ImportStudentsDialog(_classes);
        dialog.XamlRoot = this.XamlRoot;

        var result = await dialog.ShowAsync();
        
        if (result == ContentDialogResult.Primary && dialog.ImportedStudentNames.Count > 0)
        {
            // 刷新学生列表
            if (ClassComboBox.SelectedItem is ComboBoxItem item)
            {
                var classId = (int)item.Tag;
                await LoadStudentsAsync(classId);
            }
            
            var message = $"成功导入 {dialog.ImportedStudentNames.Count} 名学生";
            var resultDialog = new ContentDialog
            {
                Title = "导入完成",
                Content = message,
                CloseButtonText = "确定",
                XamlRoot = this.XamlRoot
            };
            await resultDialog.ShowAsync();
        }
    }

    private void RollCallButton_Click(object sender, RoutedEventArgs e)
    {
        // 导航到随机点名页面
    }

    private void StudentListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        // 查看学生详情
    }
}

public class StudentWithClass
{
    public Student Student { get; set; } = new();
    public string ClassName { get; set; } = string.Empty;
}
