using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace ccc.Controls;

public partial class LocalVideoPlayer : System.Windows.Controls.UserControl
{
    private IntPtr _mpvCtx = IntPtr.Zero;

    public LocalVideoPlayer()
    {
        InitializeComponent();
        Loaded += LocalVideoPlayer_Loaded;
        Unloaded += LocalVideoPlayer_Unloaded;
    }

    private void LocalVideoPlayer_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_mpvCtx != IntPtr.Zero) return;

        try
        {
            // verify dll
            var dllPath = System.IO.Path.GetFullPath("mpv-2.dll");
            if (!System.IO.File.Exists(dllPath))
            {
                 StatusText.Text = "Missing mpv-2.dll";
                 StatusText.Visibility = System.Windows.Visibility.Visible;
                 return;
            }

            // 1. Create Handle
            _mpvCtx = MpvNative.mpv_create();
            if (_mpvCtx == IntPtr.Zero) throw new Exception("Failed to create MPV Context.");

            // 2. Set Options BEFORE Init
            // Embed into our Windows Forms Panel
            long wid = VideoPanel.Handle.ToInt64();
            MpvNative.mpv_set_option_string(_mpvCtx, "wid", wid.ToString());
            
            MpvNative.mpv_set_option_string(_mpvCtx, "vo", "gpu");
            MpvNative.mpv_set_option_string(_mpvCtx, "keep-open", "yes");

            // 3. Initialize
            int res = MpvNative.mpv_initialize(_mpvCtx);
            if (res < 0) throw new Exception($"MPV Init Failed: {res}");

            StatusText.Text = "Native MPV Ready.";
            StatusText.Visibility = System.Windows.Visibility.Visible; 
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Native Init Error: {ex.Message}";
            StatusText.Foreground = System.Windows.Media.Brushes.Red;
            StatusText.Visibility = System.Windows.Visibility.Visible;
        }
    }

    private void LocalVideoPlayer_Unloaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_mpvCtx != IntPtr.Zero)
        {
             MpvNative.mpv_terminate_destroy(_mpvCtx);
             _mpvCtx = IntPtr.Zero;
        }
    }

    public void Play(string filePath)
    {
        if (_mpvCtx == IntPtr.Zero) return;
        
        StatusText.Text = $"Playing: {System.IO.Path.GetFileName(filePath)}";
        StatusText.Visibility = System.Windows.Visibility.Visible;

        var safePath = filePath.Replace("\\", "/");
        MpvNative.LoadFile(_mpvCtx, safePath);
    }
    
    public void Stop()
    {
        if (_mpvCtx == IntPtr.Zero) return;
        MpvNative.mpv_command(_mpvCtx, new string[] { "stop", null });
    }
}
