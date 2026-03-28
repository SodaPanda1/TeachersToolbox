using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Storage.Pickers;
using TeachersToolbox.App.Services;

namespace TeachersToolbox.App.Views;

public sealed partial class ImportStudentsDialog : ContentDialog
{
    private readonly ExcelImportService _excelService = new();
    private string? _selectedFilePath;
    private ExcelPreviewData? _previewData;
    private int _selectedColumnIndex = 2;
    private bool _skipHeader = true;

    public List<string> ImportedStudentNames { get; private set; } = new();

    public ImportStudentsDialog()
    {
        this.InitializeComponent();
        ColumnComboBox.SelectedIndex = 1;
    }

    private async void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeFilter.Add(".xlsx");
        picker.FileTypeFilter.Add(".xls");

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            _selectedFilePath = file.Path;
            FilePathBox.Text = _selectedFilePath;
            LoadWorksheets();
        }
    }

    private void LoadWorksheets()
    {
        if (_selectedFilePath == null) return;
        try
        {
            var worksheets = _excelService.GetWorksheetNames(_selectedFilePath);
            WorksheetComboBox.ItemsSource = worksheets;
            if (worksheets.Count > 0) WorksheetComboBox.SelectedIndex = 0;
        }
        catch (Exception ex) { PreviewInfoText.Text = $"读取文件失败: {ex.Message}"; }
    }

    private void WorksheetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadPreview();
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
            RenderPreview();
            UpdateStudentCount();
            PreviewInfoText.Text = $"工作表: {worksheetName} | 共 {_previewData.RowCount} 行, {_previewData.ColumnCount} 列";
        }
        catch (Exception ex) { PreviewInfoText.Text = $"加载预览失败: {ex.Message}"; }
    }

    private void RenderPreview()
    {
        if (_previewData == null) return;
        HeaderGrid.Children.Clear();
        HeaderGrid.ColumnDefinitions.Clear();
        DataGrid.Children.Clear();
        DataGrid.RowDefinitions.Clear();
        DataGrid.ColumnDefinitions.Clear();

        for (int col = 0; col < _previewData.Headers.Count; col++)
        {
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            var headerCell = CreateCell(_previewData.Headers[col], true, col + 1);
            Grid.SetColumn(headerCell, col);
            HeaderGrid.Children.Add(headerCell);
        }

        for (int row = 0; row < _previewData.Rows.Count; row++)
        {
            DataGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int col = 0; col < _previewData.Headers.Count; col++)
            {
                var value = col < _previewData.Rows[row].Count ? _previewData.Rows[row][col] : "";
                var cell = CreateCell(value, false, col + 1);
                Grid.SetColumn(cell, col);
                Grid.SetRow(cell, row);
                DataGrid.Children.Add(cell);
            }
        }
        UpdateColumnHighlight();
    }

    private Border CreateCell(string text, bool isHeader, int columnIndex)
    {
        var border = new Border
        {
            BorderBrush = new SolidColorBrush(Microsoft.UI.Colors.LightGray),
            BorderThickness = new Thickness(0, 0, 1, 1),
            Padding = new Thickness(8, 4, 8, 4),
            Background = columnIndex == _selectedColumnIndex
                ? new SolidColorBrush(Microsoft.UI.Colors.LightYellow)
                : new SolidColorBrush(Microsoft.UI.Colors.White),
            Tag = columnIndex
        };
        var textBlock = new TextBlock
        {
            Text = text,
            FontSize = isHeader ? 13 : 12,
            FontWeight = isHeader ? Microsoft.UI.Text.FontWeights.SemiBold : Microsoft.UI.Text.FontWeights.Normal,
            TextTrimming = TextTrimming.CharacterEllipsis,
            MaxLines = 1
        };
        border.Child = textBlock;
        return border;
    }

    private void UpdateColumnHighlight()
    {
        foreach (var child in HeaderGrid.Children)
        {
            if (child is Border border && border.Tag is int colIndex)
            {
                border.Background = colIndex == _selectedColumnIndex
                    ? new SolidColorBrush(Microsoft.UI.Colors.LightYellow)
                    : new SolidColorBrush(Microsoft.UI.Colors.White);
            }
        }
        foreach (var child in DataGrid.Children)
        {
            if (child is Border border && border.Tag is int colIndex)
            {
                border.Background = colIndex == _selectedColumnIndex
                    ? new SolidColorBrush(Microsoft.UI.Colors.LightYellow)
                    : new SolidColorBrush(Microsoft.UI.Colors.White);
            }
        }
    }

    private void UpdateStudentCount()
    {
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
        catch { StudentCountText.Text = ""; }
    }

    private void ImportButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
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
            }
        }
        catch (Exception ex)
        {
            args.Cancel = true;
            PreviewInfoText.Text = $"导入失败: {ex.Message}";
        }
    }

    private void CancelButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ImportedStudentNames.Clear();
    }
}
