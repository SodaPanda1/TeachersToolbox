using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TeachersToolbox.App.Views;

public sealed partial class AssignmentsPage : Page
{
    public AssignmentsPage()
    {
        this.InitializeComponent();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        // 打开布置作业对话框
    }

    private void AssignmentListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        // 查看作业详情
    }
}
