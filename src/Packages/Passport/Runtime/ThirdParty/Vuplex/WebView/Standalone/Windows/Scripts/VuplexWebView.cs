#if UNITY_STANDALONE_WIN || (UNITY_EDITOR_WIN && UNITY_STANDALONE_WIN)

using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Immutable.Browser.Core;
using Vuplex.WebView;

public class VuplexWebView : IWindowsWebBrowserClient
{
    public event OnUnityPostMessageDelegate OnUnityPostMessage;
    private IWebView webView;

    public VuplexWebView()
    {
        webView = Web.CreateWebView();
    }

    public async UniTask Init()
    {
        await webView.Init(1, 1);

        // Listen to messages from WebView
        webView.MessageEmitted += (sender, eventArgs) =>
        {
            OnUnityPostMessage?.Invoke(eventArgs.Value);
        };
    }

    public void LoadUrl(string url)
    {
        webView.LoadUrl(url);
    }

    public async void ExecuteJavaScript(string js)
    {
        await webView.ExecuteJavaScript(js);
    }

    public string GetPostMessageApiCall()
    {
        return "window.vuplex.postMessage";
    }

    public void Dispose()
    {
        webView.Dispose();
    }
}

#endif