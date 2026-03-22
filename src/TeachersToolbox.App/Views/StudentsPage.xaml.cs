using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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
