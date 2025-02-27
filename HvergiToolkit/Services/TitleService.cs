using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HvergiToolkit.Services;

public class TitleService
{
    public event EventHandler<String>? TitleChanged; 

    
    protected virtual void OnTitleChanged(String title)
    {
        EventHandler<String>? handler = TitleChanged;
        if(handler != null)
        {
            handler(this, title);
        }
    }

    
    //Called when anypage whishs to change the titlebar title
    public void ChangeTitle(String title)
    {
        OnTitleChanged(title);
    }

}
