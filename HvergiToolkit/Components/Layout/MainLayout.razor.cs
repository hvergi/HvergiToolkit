using HvergiToolkit.Services;
using Microsoft.VisualBasic;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HvergiToolkit.Components.Layout;

public partial class MainLayout
{
    protected override void OnInitialized()
    {
        titleservice.ChangeTitle("Core");
    }


}
