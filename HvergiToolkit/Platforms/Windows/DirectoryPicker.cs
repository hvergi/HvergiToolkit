using HvergiToolkit.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace HvergiToolkit.Platforms.Windows;

public class DirectoryPicker : IDirectoryPicker
{
    public async Task<string> GetDirectoryPathAsync()
    {
        var folderPicker = new FolderPicker();
        folderPicker.FileTypeFilter.Add("*");
        var tmp = App.Current?.Windows[0].Handler.PlatformView;
        if(tmp == null)
        {
            return string.Empty;
        }
        var hwnd = ((MauiWinUIWindow)tmp).WindowHandle;
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
        var result = folderPicker.PickSingleFolderAsync();
        await result;
        
        return result.GetResults() != null ? result.GetResults().Path : String.Empty;
    }
}
