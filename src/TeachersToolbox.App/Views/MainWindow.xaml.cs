using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Windowing;

namespace TeachersToolbox.App.Views;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.ExtendsContentIntoTitleBar = true;
        this.Title = "教师工具箱";

        // 获取显示器信息并设置适当的窗口大小
        InitializeWindowSize();
    }

    private void InitializeWindowSize()
    {
        var appWindow = this.AppWindow;
        if (appWindow != null)
        {
            // 获取显示区域
            var displayArea = DisplayArea.GetFromWindowId(
                appWindow.Id,
                DisplayAreaFallback.Primary);

            if (displayArea != null)
            {
                // 计算合适的窗口大小（屏幕的80%或最大1280x800）
                var screenWidth = displayArea.WorkArea.Width;
                var screenHeight = displayArea.WorkArea.Height;

                // 计算目标大小，考虑DPI缩放
                var targetWidth = Math.Min(1280, (int)(screenWidth * 0.8));
                var targetHeight = Math.Min(800, (int)(screenHeight * 0.8));

                // 确保最小窗口大小
                targetWidth = Math.Max(targetWidth, 800);
                targetHeight = Math.Max(targetHeight, 600);

                // 设置窗口大小
                appWindow.Resize(new Windows.Graphics.SizeInt32(targetWidth, targetHeight));

                // 将窗口居中
                var centerX = (screenWidth - targetWidth) / 2 + displayArea.WorkArea.X;
                var centerY = (screenHeight - targetHeight) / 2 + displayArea.WorkArea.Y;
                appWindow.Move(new Windows.Graphics.PointInt32(centerX, centerY));
            }
            else
            {
                // 回退到默认大小
                appWindow.Resize(new Windows.Graphics.SizeInt32(1280, 800));
            }
        }
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
