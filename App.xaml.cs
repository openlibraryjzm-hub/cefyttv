using System.Configuration;
using System.Data;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
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

    public static Services.Database.SqliteService SqliteService { get; private set; }
    public static Services.PlaylistService PlaylistService { get; private set; }
    public static Services.ConfigService ConfigService { get; private set; }
    public static Services.NavigationService NavigationService { get; private set; }
    public static Services.FolderService FolderService { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize Services
        ConfigService = new Services.ConfigService();
        await ConfigService.LoadAsync();

        SqliteService = new Services.Database.SqliteService();
        await SqliteService.InitializeAsync();

        NavigationService = new Services.NavigationService();
        PlaylistService = new Services.PlaylistService(SqliteService);
        FolderService = new Services.FolderService(SqliteService, ConfigService);

        // Initialize CefSharp settings
        var settings = new CefSharp.Wpf.CefSettings();
        // Save cookies/cache to %LocalAppData%/ccc/Cache
        settings.CachePath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ccc",
            "Cache"
        );

        // Optimize performance
        settings.CefCommandLineArgs.Add("disable-gpu-shader-disk-cache", "1");

        CefSharp.Cef.Initialize(settings);
    }
}
