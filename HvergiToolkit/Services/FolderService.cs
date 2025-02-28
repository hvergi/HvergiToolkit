using HvergiToolkit.Data;
using Microsoft.Extensions.Primitives;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace HvergiToolkit.Services;

public class FolderService
{
    public string WurmOnlinePath { get; private set; } = string.Empty;
    public string WurmSteamPath { get; private set; } = string.Empty;

    public string OnlineStatus {  get; private set; } = string.Empty;
    public string SteamStatus { get; private set; } = string.Empty;

    public bool IsWurmOnlinePathVaild { get; private set; } = false;
    public bool IsWurmSteamPathVaild { get; private set; } = false ;
    public bool IsAtleastOnePathValid { get { return (IsWurmSteamPathVaild || IsWurmOnlinePathVaild); } }

    private bool isFirstLuanch = true;

    private readonly string FolderFile = Path.Combine(HTConstants.AppDataFolder, "GamePaths.sav");

    public FolderService()
    {
        if (!Directory.Exists(HTConstants.AppDataFolder))
        {
            Directory.CreateDirectory(HTConstants.AppDataFolder);
        }
        if (File.Exists(FolderFile))
        {
            LoadFolders();
            return;
        }
        SetupSteamPath(AttemptFindSteam());
        SetupOnlinePath(AttemptFindOnline());
        SaveFolders();
    }

    public void SetupOnlinePath(string path)
    {
        PathData pathData = isPathValid(path);
        IsWurmOnlinePathVaild = pathData.isPathValid;
        OnlineStatus = pathData.message;
        WurmOnlinePath = pathData.isPathValid ? path : string.Empty;
        if(pathData.isPathValid) { SaveFolders(); }
    }

    public void SetupSteamPath(string path)
    {
        PathData pathData = isPathValid(path);
        IsWurmSteamPathVaild = pathData.isPathValid;
        SteamStatus = pathData.message;
        WurmSteamPath = pathData.isPathValid ? path : string.Empty;
        if (pathData.isPathValid) { SaveFolders(); }
    }

    private PathData isPathValid(string path)
    {
        PathData pathData = new PathData();
        pathData.isPathValid = false;
        if (path == String.Empty)
        {
            pathData.message = "No Path Entered";
        }
        else if (!Directory.Exists(path))
        {
            pathData.message = "Unable to find " + path;
        }
        else if (!Directory.Exists(Path.Combine(path, "players")))
        {
            pathData.message = "No player folder found in path " + path;
        }
        else
        {
            pathData.message = string.Empty;
            pathData.isPathValid = true;
        }
        return pathData;
    }

    private void SaveFolders()
    {
        SaveData saveData = new SaveData();
        saveData.Steam = WurmSteamPath;
        saveData.Online = WurmOnlinePath;
        File.WriteAllText(FolderFile, JsonConvert.SerializeObject(saveData));
    }
    private void LoadFolders()
    {
        if (!File.Exists(FolderFile)){ return; }
        string data = File.ReadAllText(FolderFile);
        if(data  == null || data.Length==0) { return; }
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(data);
        SetupOnlinePath(saveData.Online);
        SetupSteamPath(saveData.Steam);
    }


    public string AttemptFindSteam()
    {
        RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam");
        if (key == null)
        {
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");
        }
        if (key == null) { return string.Empty; }
        
        var tpath = key.GetValue("InstallPath"); 
        if (tpath == null) { return string.Empty;}
        
        string? SteamInstallPath = tpath?.ToString();
        if (SteamInstallPath == null) { return string.Empty ; }
        
        string libfolder = Path.Combine(SteamInstallPath, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(libfolder)) { return string.Empty; }
        
        string[] lines = File.ReadAllLines(libfolder);
        List<string> result = new List<string>();
        foreach (string line in lines)
        {
            if (line.Contains("path"))
            {
                var tmp = line.Split('"');
                foreach (string s in tmp)
                {
                    if (String.IsNullOrWhiteSpace(s) || s.Contains("path")) { continue; }
                    result.Add(s.Replace("\\\\","\\"));
                }
            }
        }
        string path = string.Empty;
        string folder = string.Empty;
        foreach (string s in result)
        {
            string tmp = Path.Combine(s, "steamapps", "appmanifest_1179680.acf");
            if (File.Exists(tmp)){
                folder = s;
                path = tmp;
                break;
            }
        }
        if(path == string.Empty) {  return string.Empty; }
        lines = File.ReadAllLines(path);
        
        result = new List<string>();
        foreach (string line in lines)
        {
            if (line.Contains("installdir"))
            {
                var tmp = line.Split("\"");
                foreach (string s in tmp)
                {
                    if (String.IsNullOrWhiteSpace(s) || s.Contains("installdir")) { continue; }
                    result.Add(s);
                }
            }
        }
        foreach (string line in result)
        {
            var tmp = Path.Combine(folder, "steamapps", "common", line);
            if (Directory.Exists(tmp))
            {
                tmp = Path.Combine(tmp, "gamedata");
                if (Directory.Exists(tmp)) { return tmp; }
            }
        }


        return String.Empty;
    }

    public string AttemptFindOnline()
    {

        RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\JavaSoft\Prefs\com\wurmonline\client");
        if (key == null) {  return string.Empty; }
        var tpath = key.GetValue("wurm_dir");
        if (tpath == null) { return string.Empty;}
        var path = tpath?.ToString();
        if(path == null) { return string.Empty; }
        if (Directory.Exists(path)) { return path; }
        //Decode Java Preferances Encoding Rules
        var path2 = toJavaValueString(stringToByteArray(path));
        if(Directory.Exists(path2)) { return path2; }
        return String.Empty;
    }

    private string toJavaValueString(byte[] windowsNameArray)
    {
        string windowsName = byteArrayToString(windowsNameArray);
        StringBuilder javaName = new StringBuilder();
        char ch;
        for (int i = 0; i < windowsName.Length; i++)
        {
            if ((ch = windowsName.ElementAt(i)) == '/')
            {
                char next = ' ';

                if (windowsName.Length > i + 1 &&
                        (next = windowsName.ElementAt(i + 1)) == 'u')
                {
                    if (windowsName.Length < i + 6)
                    {
                        break;
                    }
                    else
                    {
                        ch = (char)Int32.Parse(
                                windowsName.Substring(i + 2, i + 6), System.Globalization.NumberStyles.HexNumber);
                        i += 5;
                    }
                }
                else
                if ((windowsName.Length > i + 1) &&
                        ((windowsName.ElementAt(i + 1)) >= 'A') &&
                        (next <= 'Z'))
                {
                    ch = next;
                    i++;
                }
                else if ((windowsName.Length > i + 1) &&
                        (next == '/'))
                {
                    ch = '\\';
                    i++;
                }
            }
            else if (ch == '\\')
            {
                ch = '/';
            }
            javaName.Append(ch);
        }
        return javaName.ToString();
    }

    private byte[] toWindowsValueString(String javaName)
    {
        StringBuilder windowsName = new StringBuilder();
        for (int i = 0; i < javaName.Length; i++)
        {
            char ch = javaName.ElementAt(i);
            if ((ch < 0x0020) || (ch > 0x007f))
            {
                // write \udddd
                windowsName.Append("/u");
                String hex = $"javaName.ElementAt(i)";
                StringBuilder hex4 = new StringBuilder(new string(hex.Reverse().ToArray()));
                int len = 4 - hex4.Length;
                for (int j = 0; j < len; j++)
                {
                    hex4.Append('0');
                }
                for (int j = 0; j < 4; j++)
                {
                   
                    windowsName.Append(hex4.ToString().ElementAt(3 - j));
                }
            }
            else if (ch == '\\')
            {
                windowsName.Append("//");
            }
            else if (ch == '/')
            {
                windowsName.Append('\\');
            }
            else if ((ch >= 'A') && (ch <= 'Z'))
            {
                windowsName.Append('/').Append(ch);
            }
            else
            {
                windowsName.Append(ch);
            }
        }
        return stringToByteArray(windowsName.ToString());
    }

    private byte[] stringToByteArray(String str)
    {
        byte[] result = new byte[str.Length + 1];
        for (int i = 0; i < str.Length; i++)
        {
            result[i] = (byte)str.ElementAt(i);
        }
        result[str.Length] = 0;
        return result;
    }

    private String byteArrayToString(byte[] array)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < array.Length - 1; i++)
        {
            result.Append((char)array[i]);
        }
        return result.ToString();
    }


    private struct PathData
    {
        public string message;
        public bool isPathValid;
    }

    private struct SaveData
    {
        public string Online;
        public string Steam;
    }

}
