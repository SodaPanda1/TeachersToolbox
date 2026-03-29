using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using TeachersToolbox.Data.Repositories;

namespace TeachersToolbox.App.Views;

public sealed partial class HomePage : Page
{
    private readonly ClassRepository _classRepository;
    private readonly StudentRepository _studentRepository;

    public HomePage()
    {
        this.InitializeComponent();
        _classRepository = App.Services.GetRequiredService<ClassRepository>();
        _studentRepository = App.Services.GetRequiredService<StudentRepository>();
        this.Loaded += Page_Loaded;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadStatisticsAsync();
    }

    private async Task LoadStatisticsAsync()
    {
        try
        {
            var classes = await _classRepository.GetAllAsync();
            var classCount = classes.Count;
            
            int studentCount = 0;
            foreach (var cls in classes)
            {
                studentCount += await _studentRepository.GetCountByClassIdAsync(cls.Id);
            }

            ClassCountText.Text = classCount.ToString();
            StudentCountText.Text = studentCount.ToString();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载统计数据失败: {ex.Message}");
        }
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
            "students" => typeof(StudentsPage),
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
