using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TeachersToolbox.App.Views;

public sealed partial class AdminPage : Page
{
    public AdminPage()
    {
        this.InitializeComponent();
    }

    private void ToolItem_Click(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Border border)
        {
            var tag = border.Tag?.ToString();
            // 根据 tag 导航到对应的功能页面
        }
    }
}
