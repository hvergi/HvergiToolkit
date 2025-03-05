using HvergiToolkit.Data;
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
        public string PlayerLogFolder { get; set; }


        public PlayerModel(string playerPath)
        {
            PlayerPath = playerPath;
            PlayerLogFolder = Path.Combine(PlayerPath, "logs");
            PlayerName = Path.GetFileName(playerPath) ?? string.Empty;
        }

        public string GetLogFilePath(HTConstants.LogTypes logType)
        {
            string logFilePath = Path.Combine(PlayerLogFolder, $"{HTConstants.LogNames[(int)logType]}.{DateTime.Now:yyyy}-{DateTime.Now:MM}-{DateTime.Now:dd}.txt");
            if(File.Exists(logFilePath)) { return logFilePath; }
            logFilePath = Path.Combine(PlayerLogFolder, $"{HTConstants.LogNames[(int)logType]}.{DateTime.Now:yyyy}-{DateTime.Now:MM}.txt");
            if (File.Exists(logFilePath)) { return logFilePath; }
            logFilePath = Path.Combine(PlayerLogFolder, $"{HTConstants.LogNames[(int)logType]}.txt");
            if (File.Exists(logFilePath)) { return logFilePath; }
            return string.Empty;
        }
    }
}
