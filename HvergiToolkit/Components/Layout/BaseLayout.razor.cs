
namespace HvergiToolkit.Components.Layout;

public partial class BaseLayout
{
    

    bool IsLight { get; set; }
    public string Subtitle { get; set; } = string.Empty;
    void OnChange(bool value)
    {
        Preferences.Set("IsLight", value);
        IsLight = value;
    }

    protected override void OnInitialized()
    {
        IsLight = Preferences.Get("IsLight", true);
        titleservice.TitleChanged += HandleTitleChanged;
    }

    private void HandleTitleChanged(object? sender, string e)
    {
       Subtitle = e;
    }

}
