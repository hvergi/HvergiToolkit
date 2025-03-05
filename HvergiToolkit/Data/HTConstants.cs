using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HvergiToolkit.Data;

public class HTConstants
{
    public static readonly string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"HvergiToolkit");
    public enum LogTypes { EVENT, SKILL, COMBAT, TRADE }

    public static readonly string[] LogNames = ["_Event","_Skills","_Combat","Trade"];
}
