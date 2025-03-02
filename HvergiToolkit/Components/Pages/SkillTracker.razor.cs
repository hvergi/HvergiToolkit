using HvergiToolkit.Pages;

namespace HvergiToolkit.Components.Pages;

public partial class SkillTracker
{
    private void OpenSkillTracker()
    {
        App.Current?.OpenWindow(new Window(new SkillTrackerPage()) { Title = "Skill Tracker" });
    }
}