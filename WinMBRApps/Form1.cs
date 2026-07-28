using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace WinMBRApps
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            
            await webView21.EnsureCoreWebView2Async(null);

            
            webView21.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            webView21.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;

            
            webView21.CoreWebView2.Navigate("https://app.local/index.html");
        }

        private void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var uri = new Uri(e.Request.Uri);
            string filename = uri.AbsolutePath.TrimStart('/');
            if (string.IsNullOrEmpty(filename)) filename = "index.html";
            string resourceName = $"WinMBRApps.Apps.{filename}";
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                string mimeType = "text/html";
                if (filename.EndsWith(".css")) mimeType = "text/css";
                else if (filename.EndsWith(".js")) mimeType = "application/javascript";
                else if (filename.EndsWith(".png")) mimeType = "image/png";

                e.Response = webView21.CoreWebView2.Environment.CreateWebResourceResponse(
                    stream, 200, "OK", $"Content-Type: {mimeType}"
                );
            }
        }
    }
}