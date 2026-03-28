using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;

namespace TeachersToolbox.App.Views;

public sealed partial class MainWindow : Window
{
    private const int DefaultWidth = 1280;
    private const int DefaultHeight = 800;
    private const int MinWidth = 800;
    private const int MinHeight = 600;

    public MainWindow()
    {
        this.InitializeComponent();
        this.ExtendsContentIntoTitleBar = true;
        this.Title = "教师工具箱";

        InitializeWindowSize();
    }

    private void InitializeWindowSize()
    {
        var appWindow = this.AppWindow;
        if (appWindow == null) return;

        var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary);
        if (displayArea == null)
        {
            appWindow.Resize(new Windows.Graphics.SizeInt32(DefaultWidth, DefaultHeight));
            return;
        }

        var workArea = displayArea.WorkArea;
        
        // 计算窗口大小 - 使用屏幕的70%，但不超过默认值
        var targetWidth = Math.Min(DefaultWidth, (int)(workArea.Width * 0.7));
        var targetHeight = Math.Min(DefaultHeight, (int)(workArea.Height * 0.7));

        // 确保不小于最小尺寸
        targetWidth = Math.Max(targetWidth, MinWidth);
        targetHeight = Math.Max(targetHeight, MinHeight);

        appWindow.Resize(new Windows.Graphics.SizeInt32(targetWidth, targetHeight));

        // 居中窗口
        var centerX = (workArea.Width - targetWidth) / 2 + workArea.X;
        var centerY = (workArea.Height - targetHeight) / 2 + workArea.Y;
        appWindow.Move(new Windows.Graphics.PointInt32(centerX, centerY));
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(typeof(HomePage));
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
            return;
        }

        if (args.InvokedItemContainer is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            switch (tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;
                case "students":
                    ContentFrame.Navigate(typeof(StudentsPage));
                    break;
                case "rollcall":
                    ContentFrame.Navigate(typeof(RollCallPage));
                    break;
                case "scores":
                    ContentFrame.Navigate(typeof(ScoresPage));
                    break;
                case "timer":
                    ContentFrame.Navigate(typeof(TimerPage));
                    break;
                case "classroom":
                    ContentFrame.Navigate(typeof(ClassroomPage));
                    break;
                case "assignments":
                    ContentFrame.Navigate(typeof(AssignmentsPage));
                    break;
                case "admin":
                    ContentFrame.Navigate(typeof(AdminPage));
                    break;
            }
        }
    }
}
