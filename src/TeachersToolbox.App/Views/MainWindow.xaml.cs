using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Windowing;
using System.Text.Json;

namespace TeachersToolbox.App.Views;

public sealed partial class MainWindow : Window
{
    private const int DefaultWidth = 1280;
    private const int DefaultHeight = 800;
    private const int MinWidth = 800;
    private const int MinHeight = 600;
    private static readonly string SettingsPath = System.IO.Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "settings.json");

    public static int SelectedClassIdForRollCall { get; set; } = -1;

    public MainWindow()
    {
        this.InitializeComponent();
        this.ExtendsContentIntoTitleBar = true;
        this.Title = "教师工具箱";

        // 加载并应用保存的主题设置
        LoadAndApplyTheme();

        InitializeWindowSize();
        ContentFrame.Navigated += ContentFrame_Navigated;
    }

    private void LoadAndApplyTheme()
    {
        var theme = LoadThemeFromSettings();
        ApplyTheme(theme);
    }

    private string LoadThemeFromSettings()
    {
        try
        {
            if (System.IO.File.Exists(SettingsPath))
            {
                var json = System.IO.File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                return settings?.Theme ?? "Default";
            }
        }
        catch { }
        return "Default";
    }

    public void ApplyTheme(string theme)
    {
        var elementTheme = theme switch
        {
            "Light" => ElementTheme.Light,
            "Dark" => ElementTheme.Dark,
            _ => ElementTheme.Default
        };

        // 设置窗口根元素的主题
        if (Content is FrameworkElement root)
        {
            root.RequestedTheme = elementTheme;
        }

        // 设置 NavigationView 的主题
        NavView.RequestedTheme = elementTheme;
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
        
        var targetWidth = Math.Min(DefaultWidth, (int)(workArea.Width * 0.7));
        var targetHeight = Math.Min(DefaultHeight, (int)(workArea.Height * 0.7));

        targetWidth = Math.Max(targetWidth, MinWidth);
        targetHeight = Math.Max(targetHeight, MinHeight);

        appWindow.Resize(new Windows.Graphics.SizeInt32(targetWidth, targetHeight));

        var centerX = (workArea.Width - targetWidth) / 2 + workArea.X;
        var centerY = (workArea.Height - targetHeight) / 2 + workArea.Y;
        appWindow.Move(new Windows.Graphics.PointInt32(centerX, centerY));
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(typeof(HomePage));
        NavView.SelectedItem = NavView.MenuItems[0];
        UpdateBackButtonState();
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        UpdateBackButtonState();
        
        if (e.SourcePageType == typeof(HomePage))
            SetSelectedNavItem("home");
        else if (e.SourcePageType == typeof(StudentsPage))
            SetSelectedNavItem("students");
        else if (e.SourcePageType == typeof(RollCallPage))
            SetSelectedNavItem("rollcall");
    }

    private void SetSelectedNavItem(string tag)
    {
        foreach (var item in NavView.MenuItems)
        {
            if (item is NavigationViewItem navItem && navItem.Tag?.ToString() == tag)
            {
                NavView.SelectedItem = navItem;
                return;
            }
        }
    }

    private void UpdateBackButtonState()
    {
        NavView.IsBackEnabled = ContentFrame.CanGoBack;
    }

    private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (ContentFrame.CanGoBack)
        {
            ContentFrame.GoBack();
        }
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
            }
        }
    }
}

public class AppSettings
{
    public string Theme { get; set; } = "Default";
}
