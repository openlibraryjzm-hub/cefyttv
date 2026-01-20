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
    public static Services.Audio.AudioVisualizerService AudioVisualizerService { get; private set; } = null!;

    public static Microsoft.Web.WebView2.Core.CoreWebView2Environment? WebEnv { get; private set; }

    public static async System.Threading.Tasks.Task EnsureWebViewEnvironmentAsync()
    {
        if (WebEnv != null) return;

        var userDataFolder = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "CCC_WebView_Profile");
        var options = new Microsoft.Web.WebView2.Core.CoreWebView2EnvironmentOptions();
        options.AreBrowserExtensionsEnabled = true;

        WebEnv = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
    }

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

        AudioVisualizerService = new Services.Audio.AudioVisualizerService();
        
        // Warm up WebView2 Env
        await EnsureWebViewEnvironmentAsync();

        // Launch Window NOW that services are ready
        base.OnStartup(e);
        new MainWindow().Show();
    }
}
