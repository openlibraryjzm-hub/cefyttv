using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace ccc;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public App()
    {
        InitializeComponent();
    }

    public static Services.Database.SqliteService SqliteService { get; private set; } = null!;
    public static Services.PlaylistService PlaylistService { get; private set; } = null!;
    public static Services.ConfigService ConfigService { get; private set; } = null!;
    public static Services.NavigationService NavigationService { get; private set; } = null!;
    public static Services.FolderService FolderService { get; private set; } = null!;
    public static Services.TabService TabService { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        // Initialize Services (MUST be done before base.OnStartup loads MainWindow)
        ConfigService = new Services.ConfigService();
        await ConfigService.LoadAsync();

        SqliteService = new Services.Database.SqliteService();
        await SqliteService.InitializeAsync();

        NavigationService = new Services.NavigationService();
        PlaylistService = new Services.PlaylistService(SqliteService);
        FolderService = new Services.FolderService(SqliteService, ConfigService);
        TabService = new Services.TabService();
        await TabService.LoadConfigAsync();

        // Initialize CefSharp settings
        
        // Launch Window NOW that services are ready
        base.OnStartup(e);
        new MainWindow().Show();
        
        // Launch Window NOW that services are ready
        base.OnStartup(e);
        new MainWindow().Show();
    }
}
