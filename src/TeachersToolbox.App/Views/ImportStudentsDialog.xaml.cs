using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Storage.Pickers;
using TeachersToolbox.App.Services;
using TeachersToolbox.Core.Models;
using TeachersToolbox.Data.Repositories;

namespace TeachersToolbox.App.Views;

public sealed partial class ImportStudentsDialog : ContentDialog
{
    private readonly ExcelImportService _excelService = new();
    private readonly StudentRepository _studentRepository;
    private readonly List<Class> _classes;
    private string? _selectedFilePath;
    private ExcelPreviewData? _previewData;
    private int _selectedColumnIndex = 2;
    private bool _skipHeader = true;

    public List<string> ImportedStudentNames { get; private set; } = new();

    public ImportStudentsDialog(List<Class> classes)
    {
        InitializeComponent();
        _classes = classes;
        _studentRepository = App.Services.GetRequiredService<StudentRepository>();
        
        LoadClassList();
        
        this.Loaded += (s, e) =>
        {
            ColumnComboBox.SelectedIndex = 1;
        };
        
        this.PrimaryButtonClick += OnPrimaryButtonClick;
        this.SecondaryButtonClick += OnSecondaryButtonClick;
    }

    private void LoadClassList()
    {
        TargetClassComboBox.Items.Clear();
        foreach (var cls in _classes)
        {
            TargetClassComboBox.Items.Add(new ComboBoxItem { Content = cls.Name, Tag = cls.Id });
        }
        
        if (TargetClassComboBox.Items.Count > 0)
        {
            TargetClassComboBox.SelectedIndex = 0;
        }
    }

    private async void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".xlsx");
            picker.FileTypeFilter.Add(".xls");

            var mainWindow = App.MainWindow;
            if (mainWindow != null)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            }

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _selectedFilePath = file.Path;
                FilePathBox.Text = _selectedFilePath;
                LoadWorksheets();
            }
        }
        catch (Exception ex)
        {
            PreviewInfoText.Text = $"选择文件失败: {ex.Message}";
        }
    }

    private void LoadWorksheets()
    {
        if (_selectedFilePath == null) return;
        try
        {
            var worksheets = _excelService.GetWorksheetNames(_selectedFilePath);
            WorksheetComboBox.ItemsSource = worksheets;
            if (worksheets.Count > 0)
            {
                WorksheetComboBox.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            PreviewInfoText.Text = $"读取文件失败: {ex.Message}";
        }
    }

    private void WorksheetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (WorksheetComboBox.SelectedItem != null)
        {
            LoadPreview();
        }
    }

    private void ColumnComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ColumnComboBox.SelectedItem is ComboBoxItem item)
        {
            var column = item.Content?.ToString();
            if (column != null)
            {
                _selectedColumnIndex = ExcelImportService.GetColumnIndex(column);
                UpdatePreviewHighlight();
                UpdateStudentCount();
            }
        }
    }

    private void SkipHeaderCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        _skipHeader = SkipHeaderCheckBox.IsChecked == true;
        UpdateStudentCount();
    }

    private void LoadPreview()
    {
        if (_selectedFilePath == null || WorksheetComboBox.SelectedItem == null) return;
        try
        {
            var worksheetName = WorksheetComboBox.SelectedItem.ToString()!;
            _previewData = _excelService.GetPreviewData(_selectedFilePath, worksheetName);
            RenderPreview();
            UpdateStudentCount();
            PreviewInfoText.Text = $"工作表: {worksheetName} | 共 {_previewData.RowCount} 行, {_previewData.ColumnCount} 列";
        }
        catch (Exception ex)
        {
            PreviewInfoText.Text = $"加载预览失败: {ex.Message}";
        }
    }

    private void RenderPreview()
    {
        if (_previewData == null || _previewData.Rows.Count == 0)
        {
            PreviewItemsControl.ItemsSource = null;
            return;
        }

        var previewRows = new List<PreviewRow>();
        
        // 添加表头行
        var headerRow = new PreviewRow { IsHeader = true, RowIndex = 0 };
        for (int col = 0; col < _previewData.Headers.Count && col < 10; col++)
        {
            headerRow.Cells.Add(new PreviewCell 
            { 
                Text = _previewData.Headers[col],
                ColumnIndex = col + 1,
                IsHighlight = col + 1 == _selectedColumnIndex
            });
        }
        previewRows.Add(headerRow);

        // 添加数据行
        for (int row = 0; row < _previewData.Rows.Count && row < 20; row++)
        {
            var dataRow = new PreviewRow { IsHeader = false, RowIndex = row + 1 };
            for (int col = 0; col < _previewData.Headers.Count && col < 10; col++)
            {
                var value = col < _previewData.Rows[row].Count ? _previewData.Rows[row][col] : "";
                dataRow.Cells.Add(new PreviewCell 
                { 
                    Text = value ?? "",
                    ColumnIndex = col + 1,
                    IsHighlight = col + 1 == _selectedColumnIndex
                });
            }
            previewRows.Add(dataRow);
        }

        PreviewItemsControl.ItemsSource = previewRows;
    }

    private void UpdatePreviewHighlight()
    {
        if (_previewData == null) return;
        RenderPreview();
    }

    private void UpdateStudentCount()
    {
        if (StudentCountText == null) return;
        
        if (_selectedFilePath == null || WorksheetComboBox.SelectedItem == null)
        {
            StudentCountText.Text = "";
            return;
        }

        try
        {
            var worksheetName = WorksheetComboBox.SelectedItem.ToString()!;
            var names = _excelService.ReadStudentNames(_selectedFilePath, worksheetName, _selectedColumnIndex, _skipHeader);
            StudentCountText.Text = $"预计导入 {names.Count} 名学生";
        }
        catch
        {
            StudentCountText.Text = "";
        }
    }

    private async void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (TargetClassComboBox.SelectedItem is not ComboBoxItem classItem)
        {
            args.Cancel = true;
            PreviewInfoText.Text = "请选择目标班级";
            return;
        }
        
        var classId = (int)classItem.Tag;

        if (_selectedFilePath == null || WorksheetComboBox.SelectedItem == null)
        {
            args.Cancel = true;
            return;
        }

        try
        {
            var worksheetName = WorksheetComboBox.SelectedItem.ToString()!;
            ImportedStudentNames = _excelService.ReadStudentNames(_selectedFilePath, worksheetName, _selectedColumnIndex, _skipHeader);

            if (ImportedStudentNames.Count == 0)
            {
                args.Cancel = true;
                PreviewInfoText.Text = "未找到有效的学生姓名";
                return;
            }

            var seatNumber = 1;
            foreach (var name in ImportedStudentNames)
            {
                var student = new Student
                {
                    Name = name,
                    ClassId = classId,
                    SeatNumber = seatNumber++
                };
                await _studentRepository.AddAsync(student);
            }
        }
        catch (Exception ex)
        {
            args.Cancel = true;
            PreviewInfoText.Text = $"导入失败: {ex.Message}";
        }
    }

    private void OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ImportedStudentNames.Clear();
    }
}

// 预览数据模型
public class PreviewRow
{
    public bool IsHeader { get; set; }
    public int RowIndex { get; set; }
    public List<PreviewCell> Cells { get; set; } = new();
}

public class PreviewCell
{
    public string Text { get; set; } = "";
    public int ColumnIndex { get; set; }
    public bool IsHighlight { get; set; }
    
    public Brush HighlightBrush => IsHighlight 
        ? new SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(51, 255, 215, 0))  // #33FFD700
        : new SolidColorBrush(Microsoft.UI.Colors.Transparent);
}
