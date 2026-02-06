using System.IO;
using System.Windows;
using LibVLCSharp.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NvrDesk.Infrastructure;
using NvrDesk.Infrastructure.Data;
using NvrDesk.Presentation.ViewModels;
using NvrDesk.Presentation.Views;

namespace NvrDesk.Presentation.App;

public partial class App : Application
{
    private ServiceProvider? serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Core.Initialize();

        var services = new ServiceCollection();
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NvrDesk", "nvrdesk.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        services.AddInfrastructure($"Data Source={dbPath}");
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        serviceProvider = services.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NvrDeskDbContext>();
        dbContext.Database.Migrate();

        var window = serviceProvider.GetRequiredService<MainWindow>();
        window.Show();
    }
}
