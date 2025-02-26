using Microsoft.VisualBasic;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace HvergiToolkit.Components.Layout
{
    public partial class MainLayout
    {
        bool IsLight { get; set; }

        bool SidebarExpanded { get; set; } = false;

        //GithubSource githubSource { get; set; } = new GithubSource("", null, false);

        Task MenuClick(MenuItemEventArgs args)
        {
            return InvokeAsync(async () =>
            {
                await Task.Delay(200);
            });
        }

        void OnChange(bool value)
        {
            Preferences.Set("IsLight", value);
            IsLight = value;
        }

        protected override void OnInitialized()
        {
            IsLight = Preferences.Get("IsLight", true);
        }
    }
}
