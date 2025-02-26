using Microsoft.UI.Xaml;
using Velopack;
using Windows.UI.Popups;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HvergiToolkit.WinUI
{
    
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        private WinUIEx.SimpleSplashScreen fss { get; set; }
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            VelopackApp.Build().Run();

            fss = SimpleSplashScreen.ShowSplashScreenImage(Path.Combine(Path.GetDirectoryName(typeof(App).Assembly.Location), "splashSplashScreen.scale-100.png"));
            Task.Run(async () =>
            {
                UpdateManager UM = new UpdateManager("C:/test/");
                if (UM.IsInstalled)
                {
                    var newVersion = UM.CheckForUpdates();
                    if (newVersion != null)
                    {
                        UM.DownloadUpdates(newVersion);
                        UM.ApplyUpdatesAndRestart(newVersion);
                    }
                }
                else
                {
                    await Task.Delay(200);
                }

            }).Wait();
            
            this.InitializeComponent();
            fss.Dispose();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
