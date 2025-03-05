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
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static HvergiToolkit.Data.HTConstants;

namespace HvergiToolkit.Components.Pages.SkillTrackerApp;

public partial class SkillTrackerApp:IDisposable
{
    [CascadingParameter]
    public SkillTrackerPage? CurrentPage { get; set; }

    List<String> skillTrackerList = new List<String>();
    private PlayerModel? selectedPlayerModel;
    private string[] logPaths = [string.Empty,string.Empty,string.Empty];
    private Dictionary<string, double> skillTicks = new Dictionary<string, double>();
    
    private bool isRunning = false;
    private bool isPaused = false;
    private string IsPausedText { get { return (isPaused? "Resume" : "Pause"); } }

    private Stopwatch stopwatch = new Stopwatch();
    private System.Timers.Timer timer = new System.Timers.Timer(1000);

    protected override void OnInitialized()
    {
        PlayersData.PlayersChanged += OnPlayersChanged;
        base.OnInitialized();
        if(CurrentPage != null)
        {
            CurrentPage.Unloaded += CurrentPage_Unloaded;
        }
        timer.Elapsed += OnTimerElapsed;
        
    }

    private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    private void OnFileRead(LogReadEventArgs args)
    {
        Debug.WriteLine(args.FilePath);
        Debug.WriteLine(logPaths[(int)LogTypes.SKILL]);
        if(args.FilePath == logPaths[(int)LogTypes.SKILL])
        {
            foreach (string line in args.Lines)
            {
                if (!line.Contains("increased by")) { continue; }
                string[] tokens = line.Split(' ');
                string skillname = string.Empty;
                for (int i = 1; i < tokens.Length; i++)
                {
                    skillname += tokens[i];
                    if (tokens[i + 1] == "increased") { break; }
                    skillname += " ";
                }
                if (!skillTicks.ContainsKey(skillname)) { skillTicks.Add(skillname, double.Parse(tokens[tokens.Length - 1])); }
            }
            StateHasChanged();
        }
        else
        {

        }
    }

    private void Start()
    {
        if(selectedPlayerModel ==  null) { return; }
        isRunning = true;
        stopwatch.Restart();
        timer.Start();
    }

    private void Pause()
    {
        if (!isPaused)
        {
            stopwatch.Stop();
            timer.Stop();
        }
        else
        {

            stopwatch.Start();
            timer.Start();
        }
        
        isPaused = !isPaused;
    }

    private void Stop()
    {
        isRunning = false;
        isPaused = false;
        stopwatch.Stop();
        timer.Stop();
    }

    private void Reset()
    {
        stopwatch.Reset();
    }


    private void OnPlayersChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
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
