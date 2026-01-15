using CefSharp;

namespace ccc.Handlers;

public class CustomLifeSpanHandler : ILifeSpanHandler
{
    private readonly Action<string> _newTabCallback;

    public CustomLifeSpanHandler(Action<string> newTabCallback)
    {
        _newTabCallback = newTabCallback;
    }

    public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
    {
        // Cancel the actual popup creation
        newBrowser = null;

        // Force the popup to load in the CURRENT browser (same tab)
        chromiumWebBrowser.LoadUrl(targetUrl);

        return true; 
    }

    // Default implementations for other interface members
    public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser) { }
    public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser) { }
    public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser) { return false; }
}
