using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HvergiToolkit.Models
{
    public class PlayerModel
    {
        public string PlayerPath { get; set; }
        public string PlayerName { get; set; }

        public PlayerModel(string playerPath)
        {
            PlayerPath = playerPath;
            PlayerName = Path.GetFileName(playerPath) ?? string.Empty;
        }
    }
}
