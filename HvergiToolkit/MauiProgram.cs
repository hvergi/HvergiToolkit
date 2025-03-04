﻿using HvergiToolkit.Interfaces;
using HvergiToolkit.Platforms.Windows;
using HvergiToolkit.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Radzen;
using Velopack;
using Windows.UI.Popups;
using WinUIEx;

namespace HvergiToolkit
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddRadzenComponents();
            // Add Personal Services
            builder.Services.AddScoped<TitleService>(); //Handles Changing the titleBar (scopped so new windows make thier own)
            builder.Services.AddSingleton<FolderService>();
            builder.Services.AddSingleton<PlayerService>();
#if WINDOWS
            builder.Services.AddTransient<IDirectoryPicker, DirectoryPicker>();
#endif

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            //Use WinUIEX to set window Properties 
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.OnWindowCreated(window =>
                    {
                        if(window.Title == "Hvergi Toolkit")
                        {
                            window.CenterOnScreen(1440, 759); //Set size and center on screen using WinUIEx extension method
                        }
                        else if(window.Title == "Updater")
                        {
                            window.CenterOnScreen(720, 200);
                            window.SetIsAlwaysOnTop(true);
                        }


                        var manager = WinUIEx.WindowManager.Get(window);
                        manager.PersistenceId = "MainWindowPersistanceId"; // Remember window position and size across runs
                    });
                });
            });
            MauiApp app = builder.Build();
            return app;
        }
    }
}
