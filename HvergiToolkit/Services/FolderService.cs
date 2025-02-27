using HvergiToolkit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HvergiToolkit.Services;

public class FolderService
{
    public string WurmOnlinePath { get; private set; } = string.Empty;
    public string WurmSteamPath { get; private set; } = string.Empty;

    public string OnlineStatus {  get; private set; } = string.Empty;
    public string SteamStatus { get; private set; } = string.Empty;

    public bool IsWurmOnlinePathVaild { get; private set; } = false;
    public bool IsWurmSteamPathVaild { get; private set; } = false ;
    public bool IsAtleastOnePathValid { get { return IsWurmSteamPathVaild || IsWurmOnlinePathVaild; } }

    private bool isFirstLuanch = true;

    public void SetupOnlinePath(string path)
    {
        PathData pathData = isPathValid(path);
        IsWurmOnlinePathVaild = pathData.isPathValid;
        OnlineStatus = pathData.message;
        WurmOnlinePath = pathData.isPathValid ? path : string.Empty;
    }

    public void SetupSteamPath(string path)
    {
        PathData pathData = isPathValid(path);
        IsWurmSteamPathVaild = pathData.isPathValid;
        SteamStatus = pathData.message;
        WurmSteamPath = pathData.isPathValid ? path : string.Empty;
    }

    private PathData isPathValid(string path)
    {
        PathData pathData = new PathData();
        pathData.isPathValid = false;
        if (path == String.Empty)
        {
            pathData.message = "No Path Entered";
        }
        else if (!Directory.Exists(path))
        {
            pathData.message = "Unable to find " + path;
        }
        else if (!Directory.Exists(Path.Combine(path, "players")))
        {
            pathData.message = "No player folder found in path " + path;
        }
        else
        {
            pathData.message = string.Empty;
            pathData.isPathValid = true;
        }
        return pathData;
    }

    public void Init()
    {
        if (!isFirstLuanch) { return; }
        isFirstLuanch = false;
        if (!Directory.Exists(HTConstants.AppDataFolder))
        {
            Directory.CreateDirectory(HTConstants.AppDataFolder);
        }
        

        //SetupOnlinePath(WurmOnlinePath);
        //SetupSteamPath(WurmSteamPath);

    }

    private struct PathData
    {
        public string message;
        public bool isPathValid;
    }

}
