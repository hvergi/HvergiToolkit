namespace HvergiToolkit.Pages;

public partial class SkillTrackerPage : ContentPage
{
	public SkillTrackerPage()
	{
		InitializeComponent();
		root.Parameters = new Dictionary<string, object?>()
		{
            {"CurrentPage",this }
        };
		
    }
}