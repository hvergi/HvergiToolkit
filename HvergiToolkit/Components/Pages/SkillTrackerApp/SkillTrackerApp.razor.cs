using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HvergiToolkit.Components.Pages.SkillTrackerApp;

public partial class SkillTrackerApp
{
    List<String> skillTrackerList = new List<String>();
    
    protected override void OnInitialized()
    {
       
        skillTrackerList.Add("Chain Armour Smithing");
        skillTrackerList.Add("Chain Armour Smithing2");
        skillTrackerList.Add("Chain Armour Smithing3");
        skillTrackerList.Add("Chain Armour Smithing4");
        skillTrackerList.Add("Chain Armour Smithing5");
        UpdateUI(0);
        PlayersData.PlayersChanged += OnPlayersChanged;
        base.OnInitialized();
    }

    private async void UpdateUI(int val)
    {
        
        val++;
        await Task.Delay(1000);
        PlayersData.PlayerModels[1].PlayerId++;
        StateHasChanged();
        UpdateUI(val);
    }

    private void OnPlayersChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    private void add()
    {
        PlayersData.PlayerModels[0].PlayerId++;
    }

    
}
