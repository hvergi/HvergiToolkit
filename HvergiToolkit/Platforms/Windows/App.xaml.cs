﻿using Microsoft.UI.Xaml;
using System;
using Velopack;
using Velopack.Sources;
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
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        
        public App()
        {
            VelopackApp.Build().Run();
            
            Task.Run(async () =>
            {
                var fss = SimpleSplashScreen.ShowSplashScreenImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "pngs", "checkforupdates.png"));
                UpdateManager UM = new UpdateManager(new GithubSource("https://github.com/hvergi/HvergiToolkit/",null,false));
                if (UM.IsInstalled)
                {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "installed.txt"), "");
                    var newVersion = UM.CheckForUpdates();
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "checked.txt"), "");
                    if (newVersion != null)
                    {
                        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "newupdate.txt"), "");
                        fss.Dispose();
                        
                        fss = SimpleSplashScreen.ShowSplashScreenImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "pngs", "downloadupdates.png"));
                        UM.DownloadUpdates(newVersion);
                        fss.Dispose();
                        UM.ApplyUpdatesAndRestart(newVersion);
                    }
                    await Task.Delay(200);
                }
                else
                {
                    await Task.Delay(200);
                }
                fss?.Dispose();

            }).Wait();
            this.InitializeComponent();
           
        }

        

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
