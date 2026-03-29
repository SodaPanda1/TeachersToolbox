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
            var selectedClassId = -1;
            if (ClassComboBox.SelectedItem is ComboBoxItem current)
            {
                selectedClassId = (int)current.Tag;
            }

            _classes = await _classRepository.GetAllAsync();
            ClassComboBox.Items.Clear();
            
            ClassComboBox.Items.Add(new ComboBoxItem { Content = "全部", Tag = -1 });
            
            foreach (var cls in _classes)
            {
                ClassComboBox.Items.Add(new ComboBoxItem { Content = cls.Name, Tag = cls.Id });
            }
            
            // 恢复之前选中的班级
            var found = false;
            for (int i = 0; i < ClassComboBox.Items.Count; i++)
            {
                if (ClassComboBox.Items[i] is ComboBoxItem item && (int)item.Tag == selectedClassId)
                {
                    ClassComboBox.SelectedIndex = i;
                    found = true;
                    break;
                }
            }
            
            if (!found)
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
            
            if (classId == -1)
            {
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
            
            StudentListView.ItemsSource = null;
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

    private async void DeleteClassButton_Click(object sender, RoutedEventArgs e)
    {
        if (ClassComboBox.SelectedItem is not ComboBoxItem item || (int)item.Tag == -1)
        {
            var dialog = new ContentDialog
            {
                Title = "提示",
                Content = "请先选择要删除的班级",
                CloseButtonText = "确定",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
            return;
        }

        var classId = (int)item.Tag;
        var className = item.Content?.ToString();

        var confirmDialog = new ContentDialog
        {
            Title = "确认删除",
            Content = $"确定要删除班级 \"{className}\" 吗？\n该班级下的所有学生数据也将被删除。",
            PrimaryButtonText = "删除",
            SecondaryButtonText = "取消",
            DefaultButton = ContentDialogButton.Secondary,
            XamlRoot = this.XamlRoot
        };

        var result = await confirmDialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            try
            {
                var students = await _studentRepository.GetByClassIdAsync(classId);
                foreach (var student in students)
                {
                    await _studentRepository.DeleteAsync(student.Id);
                }
                await _classRepository.DeleteAsync(classId);
                await LoadClassesAsync();
            }
            catch (Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "删除失败",
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
            var importedCount = dialog.ImportedStudentNames.Count;
            
            // 重新加载班级列表（以防导入了新班级）
            await LoadClassesAsync();
            
            // 获取当前选中的班级并刷新学生列表
            if (ClassComboBox.SelectedItem is ComboBoxItem item)
            {
                var classId = (int)item.Tag;
                await LoadStudentsAsync(classId);
            }
            
            var resultDialog = new ContentDialog
            {
                Title = "导入完成",
                Content = $"成功导入 {importedCount} 名学生",
                CloseButtonText = "确定",
                XamlRoot = this.XamlRoot
            };
            await resultDialog.ShowAsync();
        }
    }

    private void RollCallButton_Click(object sender, RoutedEventArgs e)
    {
        int selectedClassId = -1;
        if (ClassComboBox.SelectedItem is ComboBoxItem item)
        {
            selectedClassId = (int)item.Tag;
        }

        MainWindow.SelectedClassIdForRollCall = selectedClassId;

        var mainWindow = App.MainWindow;
        if (mainWindow != null)
        {
            var contentFrameField = mainWindow.GetType().GetField("ContentFrame", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (contentFrameField?.GetValue(mainWindow) is Frame frame)
            {
                frame.Navigate(typeof(RollCallPage));
            }
        }
    }

    private void StudentListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        // 查看学生详情（暂不实现）
    }
}

public class StudentWithClass
{
    public Student Student { get; set; } = new();
    public string ClassName { get; set; } = string.Empty;
}
