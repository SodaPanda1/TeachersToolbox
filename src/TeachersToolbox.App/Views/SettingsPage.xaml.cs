using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.Json;
using Windows.Storage.Pickers;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data.Repositories;

namespace TeachersToolbox.App.Views;

public sealed partial class SettingsPage : Page
{
    private readonly ClassRepository _classRepository;
    private readonly StudentRepository _studentRepository;
    private static readonly string SettingsPath = System.IO.Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "settings.json");

    public SettingsPage()
    {
        InitializeComponent();
        _classRepository = App.Services.GetRequiredService<ClassRepository>();
        _studentRepository = App.Services.GetRequiredService<StudentRepository>();
        this.Loaded += Page_Loaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadStatisticsAsync();
        LoadThemePreference();
    }

    private void LoadThemePreference()
    {
        var theme = LoadThemeFromSettings();
        for (int i = 0; i < ThemeComboBox.Items.Count; i++)
        {
            if (ThemeComboBox.Items[i] is ComboBoxItem item && item.Tag?.ToString() == theme)
            {
                ThemeComboBox.SelectedIndex = i;
                break;
            }
        }
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeComboBox.SelectedItem is ComboBoxItem item)
        {
            var theme = item.Tag?.ToString() ?? "Default";
            SaveThemeToSettings(theme);
            
            // 应用主题 - 使用 MainWindow 的 ApplyTheme 方法
            App.MainWindow?.ApplyTheme(theme);
        }
    }

    private string LoadThemeFromSettings()
    {
        try
        {
            if (System.IO.File.Exists(SettingsPath))
            {
                var json = System.IO.File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                return settings?.Theme ?? "Default";
            }
        }
        catch { }
        return "Default";
    }

    private void SaveThemeToSettings(string theme)
    {
        try
        {
            var settings = new AppSettings { Theme = theme };
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"保存设置失败: {ex.Message}");
        }
    }

    private async Task LoadStatisticsAsync()
    {
        try
        {
            var classes = await _classRepository.GetAllAsync();
            ClassCountText.Text = classes.Count.ToString();

            int studentCount = 0;
            foreach (var cls in classes)
            {
                studentCount += await _studentRepository.GetCountByClassIdAsync(cls.Id);
            }
            StudentCountText.Text = studentCount.ToString();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载统计数据失败: {ex.Message}");
        }
    }

    private async void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("JSON 文件", new List<string> { ".json" });
            picker.SuggestedFileName = $"teachers_toolbox_backup_{DateTime.Now:yyyyMMdd_HHmmss}";

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;

            // 获取所有数据
            var classes = await _classRepository.GetAllAsync();
            var allStudents = new List<Student>();
            foreach (var cls in classes)
            {
                var students = await _studentRepository.GetByClassIdAsync(cls.Id);
                allStudents.AddRange(students);
            }

            var exportData = new ExportData
            {
                Classes = classes,
                Students = allStudents,
                ExportDate = DateTime.Now,
                Version = "1.0.0"
            };

            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(file.Path, json);

            await ShowMessageAsync("导出成功", $"数据已导出到：{file.Path}");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync("导出失败", ex.Message);
        }
    }

    private async void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".json");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file == null) return;

            // 确认导入
            var confirm = new ContentDialog
            {
                Title = "确认导入",
                Content = "导入数据将覆盖现有数据，确定要继续吗？",
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Secondary,
                XamlRoot = this.XamlRoot
            };

            if (await confirm.ShowAsync() != ContentDialogResult.Primary) return;

            // 读取并导入数据
            var json = System.IO.File.ReadAllText(file.Path);
            var importData = JsonSerializer.Deserialize<ExportData>(json);

            if (importData == null)
            {
                await ShowMessageAsync("导入失败", "文件格式不正确");
                return;
            }

            // 导入班级和学生
            foreach (var cls in importData.Classes)
            {
                var existingClasses = await _classRepository.GetAllAsync();
                if (!existingClasses.Any(c => c.Name == cls.Name))
                {
                    await _classRepository.AddAsync(cls);
                }
            }

            await ShowMessageAsync("导入成功", $"已导入 {importData.Classes.Count} 个班级，{importData.Students.Count} 名学生");
            await LoadStatisticsAsync();
        }
        catch (Exception ex)
        {
            await ShowMessageAsync("导入失败", ex.Message);
        }
    }

    private async void BackupButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var appFolder = AppDomain.CurrentDomain.BaseDirectory;
            var dbPath = System.IO.Path.Combine(appFolder, "teachers_toolbox.db");

            if (!System.IO.File.Exists(dbPath))
            {
                await ShowMessageAsync("备份失败", "数据库文件不存在");
                return;
            }

            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("SQLite 数据库", new List<string> { ".db" });
            picker.SuggestedFileName = $"teachers_toolbox_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;

            System.IO.File.Copy(dbPath, file.Path, true);

            await ShowMessageAsync("备份成功", $"数据库已备份到：{file.Path}");
        }
        catch (Exception ex)
        {
            await ShowMessageAsync("备份失败", ex.Message);
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadStatisticsAsync();
    }

    private async Task ShowMessageAsync(string title, string content)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            CloseButtonText = "确定",
            XamlRoot = this.XamlRoot
        };
        await dialog.ShowAsync();
    }
}

// 导出数据模型
public class ExportData
{
    public List<Class> Classes { get; set; } = new();
    public List<Student> Students { get; set; } = new();
    public DateTime ExportDate { get; set; }
    public string Version { get; set; } = string.Empty;
}
