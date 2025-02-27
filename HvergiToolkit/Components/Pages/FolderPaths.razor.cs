using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Microsoft.Maui.Storage;
using HvergiToolkit.Services;

namespace HvergiToolkit.Components.Pages;

public partial class FolderPaths
{
    async void SetupSteam()
    {
        string path = await DPicker.GetDirectoryPathAsync();
        Folders.SetupSteamPath(path);
        StateHasChanged();
    }

    async void SetupWurm()
    {
        string path = await DPicker.GetDirectoryPathAsync();
        Folders.SetupOnlinePath(path);
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        Folders.Init();
    }

}
