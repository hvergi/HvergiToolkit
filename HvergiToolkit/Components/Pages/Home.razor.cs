using HvergiToolkit.Pages;

namespace HvergiToolkit.Components.Pages;

public partial class Home
{
    double Height { get; set; }
    double Width { get; set; }

    private void Open()
    {
        var window = new Window(new SkillTrackerPage()) { Title = "New Popup" };
        window.Height = 900;
        App.Current?.OpenWindow(window);
        Height = window.Height;
        Width = window.Width;

    }
}
