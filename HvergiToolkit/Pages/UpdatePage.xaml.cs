using Microsoft.UI.Composition;
using System.ComponentModel;
using Velopack;
using Velopack.Sources;
using HvergiToolkit;

namespace HvergiToolkit.Pages;

public partial class UpdatePage : ContentPage
{
	private string updatePercent = string.Empty; 
	public string UpdatePercent {
		get { return updatePercent; } 
		set{
			updatePercent = value;
			OnPropertyChanged(nameof(UpdatePercent));
		} 
	}

	private string status = "Checking for Updates.";
	public string Status
	{
		get { return status; }
		set
		{
			status = value;
			OnPropertyChanged(nameof(Status));
		}
	}

	private UpdateManager updateManager;
	private UpdateInfo? updateInfo;

	public UpdatePage()
	{
		UpdatePercent = "";
		//Local Update testing
		updateManager = new UpdateManager("C:\\source\\repos\\HvergiToolkit\\Releases");
		
		//updateManager = new UpdateManager(new GithubSource("https://github.com/hvergi/HvergiToolkit/", null, false));
        InitializeComponent();
        CheckForUpdates();
    }

	async void CheckForUpdates()
	{
		if (updateManager.IsInstalled)
		{
            updateInfo = await updateManager.CheckForUpdatesAsync();
            if (updateInfo != null)
            {
				await Task.Delay(500);
				await DownloadUpdates();
            }
        }
		await Task.Delay(1000); //Delay added to allow the update page to fully load, so it can be properly closed.
        App.Current?.OpenWindow(new Window(new MainPage()) { Title = "Hvergi Toolkit" });
		App.Current?.CloseWindow(this.Window);

    }

	 async Task DownloadUpdates()
	{
		if (updateInfo != null) {
			Status = "Downloading updates";
			await updateManager.DownloadUpdatesAsync(updateInfo, progress => UpdatePercent = $"{progress / 100.0:p}");
            Status = "Rebooting App";
            await Task.Delay(500);
            updateManager.ApplyUpdatesAndRestart(updateInfo);
		}
    }
}