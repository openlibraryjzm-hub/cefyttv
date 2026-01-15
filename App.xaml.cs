using System.Configuration;
using System.Data;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;

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

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

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
