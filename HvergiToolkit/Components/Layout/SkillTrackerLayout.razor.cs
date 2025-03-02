using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HvergiToolkit.Components.Layout;

public partial class SkillTrackerLayout
{
    string URI { get; set; } = "Test";
    protected override void OnInitialized()
    {
        titleservice.ChangeTitle("Skill Tracker");
        
    }
}
