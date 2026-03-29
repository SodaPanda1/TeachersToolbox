using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
    private bool _webViewInitialized = false;

    public List<string> ImportedStudentNames { get; private set; } = new();

    public ImportStudentsDialog(List<Class> classes)
    {
        InitializeComponent();
        _classes = classes;
        _studentRepository = App.Services.GetRequiredService<StudentRepository>();
        
        LoadClassList();
        
        this.Loaded += async (s, e) =>
        {
            ColumnComboBox.SelectedIndex = 1;
            await InitializeWebViewAsync();
        };
        
        this.PrimaryButtonClick += OnPrimaryButtonClick;
        this.SecondaryButtonClick += OnSecondaryButtonClick;
    }

    private async Task InitializeWebViewAsync()
    {
        if (_webViewInitialized) return;
        
        try
        {
            await PreviewWebView.EnsureCoreWebView2Async();
            _webViewInitialized = true;
            ShowEmptyPreview();
        }
        catch (Exception ex)
        {
            PreviewInfoText.Text = $"WebView2 初始化失败: {ex.Message}";
        }
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
                UpdateColumnHighlight();
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
            RenderPreviewToWebView();
            UpdateStudentCount();
            PreviewInfoText.Text = $"工作表: {worksheetName} | 共 {_previewData.RowCount} 行, {_previewData.ColumnCount} 列";
        }
        catch (Exception ex)
        {
            PreviewInfoText.Text = $"加载预览失败: {ex.Message}";
        }
    }

    private void ShowEmptyPreview()
    {
        var html = GenerateEmptyHtml();
        PreviewWebView.NavigateToString(html);
    }

    private void RenderPreviewToWebView()
    {
        if (_previewData == null || !_webViewInitialized) return;
        
        var html = GenerateHtmlTable(_previewData, _selectedColumnIndex);
        PreviewWebView.NavigateToString(html);
    }

    private void UpdateColumnHighlight()
    {
        if (_previewData == null || !_webViewInitialized) return;
        RenderPreviewToWebView();
    }

    private string GenerateEmptyHtml()
    {
        return """
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="utf-8">
                <style>
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    body { 
                        font-family: 'Segoe UI', Tahoma, sans-serif;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        height: 100vh;
                        background: #f5f5f5;
                        color: #666;
                    }
                </style>
            </head>
            <body>
                <div>请选择Excel文件开始预览</div>
            </body>
            </html>
            """;
    }

    private string GenerateHtmlTable(ExcelPreviewData data, int highlightColumn)
    {
        var sb = new System.Text.StringBuilder();
        
        sb.AppendLine("""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="utf-8">
                <style>
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    body { 
                        font-family: 'Segoe UI', Tahoma, sans-serif;
                        background: #fff;
                        padding: 8px;
                    }
                    table {
                        border-collapse: collapse;
                        width: 100%;
                        font-size: 12px;
                    }
                    th {
                        background: #f0f0f0;
                        font-weight: 600;
                        position: sticky;
                        top: 0;
                    }
                    td, th {
                        border: 1px solid #ddd;
                        padding: 6px 8px;
                        text-align: left;
                        white-space: nowrap;
                        min-width: 80px;
                        max-width: 150px;
                        overflow: hidden;
                        text-overflow: ellipsis;
                    }
                    tr:hover { background: #f8f8f8; }
                    .highlight { background: #fff3cd !important; }
                    .header-highlight { background: #ffe69c !important; }
                </style>
            </head>
            <body>
            """);
        
        // 添加表头
        sb.AppendLine("<table><thead><tr>");
        sb.AppendLine($"<th class='{(highlightColumn == 1 ? "header-highlight" : "")}'>A</th>");
        
        for (int col = 1; col < Math.Min(data.Headers.Count, 10); col++)
        {
            var colLetter = ExcelImportService.GetColumnLetter(col + 1);
            var highlightClass = col + 1 == highlightColumn ? "header-highlight" : "";
            sb.AppendLine($"<th class='{highlightClass}'>{colLetter}</th>");
        }
        sb.AppendLine("</tr></thead>");
        
        // 添加数据行
        sb.AppendLine("<tbody>");
        var rowCount = Math.Min(data.Rows.Count, 25);
        
        for (int row = 0; row < rowCount; row++)
        {
            sb.AppendLine("<tr>");
            for (int col = 0; col < Math.Min(data.Headers.Count, 10); col++)
            {
                var value = col < data.Rows[row].Count ? (data.Rows[row][col] ?? "") : "";
                var highlightClass = col + 1 == highlightColumn ? "highlight" : "";
                sb.AppendLine($"<td class='{highlightClass}'>{System.Net.WebUtility.HtmlEncode(value)}</td>");
            }
            sb.AppendLine("</tr>");
        }
        sb.AppendLine("</tbody></table>");
        
        // 添加底部信息
        if (data.Rows.Count > 25)
        {
            sb.AppendLine($"<div style='padding: 8px; color: #666; font-size: 11px;'>显示前 25 行，共 {data.Rows.Count} 行</div>");
        }
        
        sb.AppendLine("</body></html>");
        
        return sb.ToString();
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
