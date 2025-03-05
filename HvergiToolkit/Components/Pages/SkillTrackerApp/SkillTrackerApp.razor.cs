using HvergiToolkit.Models;
using HvergiToolkit.Pages;
using HvergiToolkit.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HvergiToolkit.Components.Pages.SkillTrackerApp;

public partial class SkillTrackerApp:IDisposable
{
    [CascadingParameter]
    public SkillTrackerPage? CurrentPage { get; set; }

    List<String> skillTrackerList = new List<String>();
    private PlayerModel? selectedPlayerModel;
    private string[] logPaths = [string.Empty,string.Empty,string.Empty];
    
    protected override void OnInitialized()
    {
        PlayersData.PlayersChanged += OnPlayersChanged;
        base.OnInitialized();
        if(CurrentPage != null)
        {
            CurrentPage.Unloaded += CurrentPage_Unloaded;
        }
        
    }

    private void OnFileRead(LogReadEventArgs args)
    {
        Debug.WriteLine(args.FilePath);
        foreach (string line in args.Lines)
        {
            Debug.WriteLine(line);
        }
    }


    private void OnPlayersChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    private void add()
    {
        if (selectedPlayerModel != null)
        {
            logPaths[0] = selectedPlayerModel.GetLogFilePath(Data.HTConstants.LogTypes.EVENT);
            LogReader.AddLogFile(logPaths[0], OnFileRead);
        }
    }
    private void stop()
    {
        Dispose();
        
    }

    private void CurrentPage_Unloaded(object? sender, EventArgs e)
    {
        Dispose();
    }


    public void Dispose()
    {
        foreach (string line in logPaths)
        {
            LogReader.RemoveLogFile(line, OnFileRead);
        }
    }
}
