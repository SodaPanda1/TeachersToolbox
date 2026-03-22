using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using TeachersToolbox.App.Views;
using TeachersToolbox.Data;
using TeachersToolbox.Data.Repositories;
using TeachersToolbox.Core.Services;

namespace TeachersToolbox.App;

public partial class App : Application
{
    private Window? _window;
    private ServiceProvider? _serviceProvider;

    public App()
    {
        this.InitializeComponent();
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        // 数据库
        var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        var dbPath = System.IO.Path.Combine(localFolder, "teachers_toolbox.db");
        services.AddSingleton(new DbContext(dbPath));

        // 仓储
        services.AddSingleton<ClassRepository>();
        services.AddSingleton<StudentRepository>();
        services.AddSingleton<ScoreRepository>();
        services.AddSingleton<AssignmentRepository>();
        services.AddSingleton<AttendanceRepository>();
        services.AddSingleton<ClassroomPointRepository>();
        services.AddSingleton<CourseRepository>();

        // 服务
        services.AddSingleton<RandomPickerService>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public static ServiceProvider Services => ((App)Current)._serviceProvider!;

    protected override async void OnLaunched(LaunchActivatedEventArgs e)
    {
        // 初始化数据库
        try
        {
            var dbContext = Services.GetRequiredService<DbContext>();
            await dbContext.InitializeDatabaseAsync();
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
        }

        _window = new MainWindow();
        _window.Activate();
    }
}
