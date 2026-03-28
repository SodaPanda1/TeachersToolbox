using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace TeachersToolbox.App.Views;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();
    }

    private void QuickAction_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string tag)
        {
            NavigateToPage(tag);
        }
    }

    private void NavigateToPage(string tag)
    {
        var targetPage = tag switch
        {
            "rollcall" => typeof(RollCallPage),
            "scores" => typeof(ScoresPage),
            "timer" => typeof(TimerPage),
            "students" => typeof(StudentsPage),
            "classroom" => typeof(ClassroomPage),
            "assignments" => typeof(AssignmentsPage),
            "admin" => typeof(AdminPage),
            _ => null
        };

        if (targetPage != null && Frame != null)
        {
            Frame.Navigate(targetPage);
        }
    }

    private void Card_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border border)
        {
            border.Opacity = 0.85;
            border.Scale = new System.Numerics.Vector3(1.02f, 1.02f, 1.0f);
        }
    }

    private void Card_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border border)
        {
            border.Opacity = 1.0;
            border.Scale = new System.Numerics.Vector3(1.0f, 1.0f, 1.0f);
        }
    }
}
