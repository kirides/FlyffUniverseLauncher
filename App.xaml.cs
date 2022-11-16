using System;
using System.IO;
using System.Windows;
using CefSharp;
using CefSharp.Handler;
using CefSharp.Wpf;

namespace FlyffUniverseLauncher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var exitCode = CefSharp.BrowserSubprocess.SelfHost.Main(e.Args);
            if (exitCode >= 0)
            {
                return;
            }
            const bool multiThreadedMessageLoop = true;
            var settings = new CefSettings();
            settings.MultiThreadedMessageLoop = multiThreadedMessageLoop;
            settings.ExternalMessagePump = !multiThreadedMessageLoop;

            Init(e.Args, settings, new BrowserProcessHandler());

            base.OnStartup(e);
        }

        public static void Init(string[] args, CefSettingsBase settings, IBrowserProcessHandler browserProcessHandler)
        {
            var instance = args.Length > 0 ? args[0] : "global";

            settings.CachePath = Path.GetFullPath($"instances\\{instance}\\cache");
            settings.UserDataPath = Path.GetFullPath($"instances\\{instance}\\userdata");

            Directory.CreateDirectory(settings.CachePath);
            Directory.CreateDirectory(settings.UserDataPath);
            // settings.RootCachePath = Path.GetFullPath($"instances\\{instance}\\rootcache");

            settings.BrowserSubprocessPath = Path.GetFullPath(typeof(App).Assembly.GetName().Name+".exe");

            settings.UserAgent = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{Cef.CefSharpVersion} Safari/537.36 Edg/{Cef.CefSharpVersion}";

            //settings.CefCommandLineArgs.Add("enable-logging"); //Enable Logging for the Renderer process (will open with a cmd prompt and output debug messages - use in conjunction with setting LogSeverity = LogSeverity.Verbose;)
            settings.LogSeverity = LogSeverity.Disable; // Needed for enable-logging to output messages

            // settings.CefCommandLineArgs.Add("disable-extensions"); //Extension support can be disabled
            settings.CefCommandLineArgs.Add("disable-pdf-extension"); //The PDF extension specifically can be disabled

            settings.CefCommandLineArgs.Add("disable-gpu-vsync"); //Disable Vsync

            // The following options control accessibility state for all frames.
            settings.CefCommandLineArgs.Add("disable-renderer-accessibility");

            //Enables Uncaught exception handler
            settings.UncaughtExceptionStackSize = 10;

            //Experimental setting see https://bitbucket.org/chromiumembedded/cef/issues/2969/support-chrome-windows-with-cef-callbacks
            //for details
            //settings.ChromeRuntime = true;

            settings.CookieableSchemesList = "custom";

            //This must be set before Cef.Initialized is called
            CefSharpSettings.FocusedNodeChangedEnabled = true;

            //Async Javascript Binding - methods are queued on TaskScheduler.Default.
            //Set this to true to when you have methods that return Task<T>
            //CefSharpSettings.ConcurrentTaskExecution = true;

            //Exit the subprocess if the parent process happens to close
            //This is optional at the moment
            //https://github.com/cefsharp/CefSharp/pull/2375/
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            bool performDependencyCheck = false;

            if (!Cef.Initialize(settings, performDependencyCheck: performDependencyCheck, browserProcessHandler: browserProcessHandler))
            {
                throw new Exception("Unable to Initialize Cef");
            }
        }
    }
}
