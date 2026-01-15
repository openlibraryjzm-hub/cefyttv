using CefSharp;
using CefSharp.Handler;

namespace ccc.Handlers;

public class CustomRequestHandler : RequestHandler
{
    protected override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
    {
        // THIS IS WHERE YOU FILTER TRAFFIC
        
        // Example: Hardcoded AdBlock
        if (request.Url.Contains("ads.google") || request.Url.Contains("doubleclick"))
        {
            // Block the request
            return true; 
        }

        // Example: Hardcoded Extension Logic
        // if (request.Url == "chrome-extension://my-fake-id/index.html") { ... }

        return false; // Allow everything else
    }
}
