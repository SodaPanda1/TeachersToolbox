using ClosedXML.Excel;

namespace TeachersToolbox.App.Services;

public class ExcelImportService
{
    /// <summary>
    /// 获取工作簿中的所有工作表名称
    /// </summary>
    public List<string> GetWorksheetNames(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        return workbook.Worksheets.Select(ws => ws.Name).ToList();
    }

    /// <summary>
    /// 获取工作表的预览数据
    /// </summary>
    public ExcelPreviewData GetPreviewData(string filePath, string worksheetName, int maxRows = 20)
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(worksheetName);

        var data = new ExcelPreviewData
        {
            WorksheetName = worksheetName,
            RowCount = worksheet.LastRowUsed()?.RowNumber() ?? 0,
            ColumnCount = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0,
            Headers = new List<string>(),
            Rows = new List<List<string>>()
        };

        // 获取列头 (使用 A, B, C... 格式)
        for (int col = 1; col <= Math.Min(data.ColumnCount, 26); col++)
        {
            data.Headers.Add(GetColumnLetter(col));
        }

        // 获取数据行
        var lastRow = Math.Min(data.RowCount, maxRows);
        for (int row = 1; row <= lastRow; row++)
        {
            var rowData = new List<string>();
            for (int col = 1; col <= Math.Min(data.ColumnCount, 26); col++)
            {
                rowData.Add(worksheet.Cell(row, col).GetString());
            }
            data.Rows.Add(rowData);
        }

        return data;
    }

    /// <summary>
    /// 从指定列读取学生姓名
    /// </summary>
    public List<string> ReadStudentNames(string filePath, string worksheetName, 
        int columnIndex, bool skipHeader)
    {
        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(worksheetName);

        var names = new List<string>();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;
        var startRow = skipHeader ? 2 : 1;

        for (int row = startRow; row <= lastRow; row++)
        {
            var name = worksheet.Cell(row, columnIndex).GetString().Trim();
            if (!string.IsNullOrWhiteSpace(name))
            {
                names.Add(name);
            }
        }

        return names;
    }

    /// <summary>
    /// 获取列字母 (1=A, 2=B, ...)
    /// </summary>
    public static string GetColumnLetter(int columnIndex)
    {
        var letter = "";
        while (columnIndex > 0)
        {
            var remainder = (columnIndex - 1) % 26;
            letter = (char)('A' + remainder) + letter;
            columnIndex = (columnIndex - 1) / 26;
        }
        return letter;
    }

    /// <summary>
    /// 获取列索引 (A=1, B=2, ...)
    /// </summary>
    public static int GetColumnIndex(string columnLetter)
    {
        var index = 0;
        foreach (var c in columnLetter.ToUpper())
        {
            index = index * 26 + (c - 'A' + 1);
        }
        return index;
    }
}

public class ExcelPreviewData
{
    public string WorksheetName { get; set; } = string.Empty;
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
    public List<string> Headers { get; set; } = new();
    public List<List<string>> Rows { get; set; } = new();
}
