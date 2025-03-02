using HvergiToolkit.Data;
using HvergiToolkit.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HvergiToolkit.Services;

public class PlayerService
{
    public List<PlayerModel> WurmPlayerModels { get; set; } = new List<PlayerModel>();
    public List<PlayerModel> SteamPlayerModels { get; set; } = new List<PlayerModel>();

    public List<PlayerModel> PlayerModels { get { return Enumerable.Concat(WurmPlayerModels, SteamPlayerModels).ToList<PlayerModel>(); } }
    public int PlayerCount { get { return WurmPlayerModels.Count + SteamPlayerModels.Count; } }

    private readonly FolderService FSInstance;

    public event EventHandler? PlayersChanged;

    public PlayerService(FolderService folderService) { 
        
        FSInstance = folderService;
        FSInstance.FoldersChanged += OnFoldersChanged;
        if (FSInstance.IsWurmOnlinePathVaild) { WurmPlayerModels = GetPlayersInPath(FSInstance.WurmOnlinePath); }
        if (FSInstance.IsWurmSteamPathVaild) { SteamPlayerModels = GetPlayersInPath(FSInstance.WurmSteamPath); }
    }

    private void OnFoldersChanged(object? sender, EventArgs e)
    {
        if (FSInstance.IsWurmOnlinePathVaild)
        {
            WurmPlayerModels = GetPlayersInPath(FSInstance.WurmOnlinePath);
        }
        else
        {
            WurmPlayerModels = new List<PlayerModel>();
        }
        if (FSInstance.IsWurmSteamPathVaild)
        {
            SteamPlayerModels = GetPlayersInPath(FSInstance.WurmSteamPath);
        }
        else
        {
            SteamPlayerModels = new List<PlayerModel>();
        }
    }

    private List<PlayerModel> GetPlayersInPath(string path)
    {
        List<PlayerModel> playerModels = new List<PlayerModel>();
        path = Path.Combine(path, "players");
        if(!Directory.Exists(path)) { return playerModels; } //Returns empty list if no players are found
        
        string[] data = Directory.EnumerateDirectories(path).ToArray();
        if(data.Length == 0) { return playerModels; } //returns empty list if no folders are inside the players folder
        foreach (string item in data)
        {
            //Checks for a logs folder as wurm makes ghost players if you use both steam and standalone
            if (Directory.Exists(Path.Combine(item, "logs"))){
                playerModels.Add(new PlayerModel(item));
            }   
        }
        PlayersChanged?.Invoke(this, EventArgs.Empty);
        return playerModels;
    }

    

}
