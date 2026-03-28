using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TeachersToolbox.App.Services;

namespace TeachersToolbox.App.Views;

public sealed partial class StudentsPage : Page
{
    public StudentsPage()
    {
        this.InitializeComponent();
    }

    private void ClassComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // 加载班级学生列表
    }

    private async void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ImportStudentsDialog();
        dialog.XamlRoot = this.XamlRoot;

        var result = await dialog.ShowAsync();
        
        if (result == ContentDialogResult.Primary && dialog.ImportedStudentNames.Count > 0)
        {
            // 显示导入结果
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

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        // 打开添加学生对话框
    }

    private void RollCallButton_Click(object sender, RoutedEventArgs e)
    {
        // 打开随机点名页面
    }

    private void StudentListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        // 查看学生详情
    }
}
