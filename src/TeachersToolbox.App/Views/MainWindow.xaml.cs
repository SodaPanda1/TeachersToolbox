using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace TeachersToolbox.App.Views;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.ExtendsContentIntoTitleBar = true;
        this.Title = "教师工具箱";
        
        var appWindow = this.AppWindow;
        if (appWindow != null)
        {
            appWindow.Resize(new Windows.Graphics.SizeInt32(1280, 800));
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
