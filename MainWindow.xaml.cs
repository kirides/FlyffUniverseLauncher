using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CefSharp;
using BrowserSettings = CefSharp.Core.BrowserSettings;

namespace FlyffUniverseLauncher;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var args = Environment.GetCommandLineArgs().Skip(1).ToArray();

        Title = args.Length > 0 ? $"Flyff Universe - {args[0]}" : "Flyff Universe";
            
        var browser = new CefSharp.Wpf.ChromiumWebBrowser()
        {
            MenuHandler = new DisabledMenuHandler(),
            BrowserSettings = new BrowserSettings()
            {
                WindowlessFrameRate = 144,
                WebGl = CefState.Enabled,
                LocalStorage = CefState.Enabled,
                Databases = CefState.Enabled,
                Javascript = CefState.Enabled,
                ImageLoading = CefState.Enabled,
                RemoteFonts = CefState.Enabled,
                JavascriptAccessClipboard = CefState.Enabled,
            },
        };
        browser.Address = "https://universe.flyff.com/play";

        Content = browser;

        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F11)
        {
            if (this.WindowStyle != WindowStyle.None)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            }
        }
    }
}