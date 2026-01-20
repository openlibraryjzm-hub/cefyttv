chrome.runtime.onInstalled.addListener(() => {
    chrome.contextMenus.create({
        id: "quick-download-ext",
        title: "⚡ Quick Download (Extended)",
        contexts: ["link", "image", "video", "audio"]
    });
});

chrome.contextMenus.onClicked.addListener((info, tab) => {
    if (info.menuItemId === "quick-download-ext") {
        const url = info.linkUrl || info.srcUrl;
        if (url) {
            // With 'saveAs' set to false and NO filename specified,
            // WebView2/Chrome should use the Content-Disposition header
            // or the URL path to determine the correct filename and extension.
            chrome.downloads.download({
                url: url,
                saveAs: false
            });
        }
    }
});
