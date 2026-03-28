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

    public static MainWindow? MainWindow { get; private set; }

    public App()
    {
        this.InitializeComponent();
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();

        // 数据库 - 使用应用程序目录
        var appFolder = AppDomain.CurrentDomain.BaseDirectory;
        var dbPath = System.IO.Path.Combine(appFolder, "teachers_toolbox.db");
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

        MainWindow = new MainWindow();
        MainWindow.Activate();
    }
}
