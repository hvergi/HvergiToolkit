using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HvergiToolkit.Services;

public class PlayerLogReaderService
{
    private Dictionary<string, LogFile> _files = new Dictionary<string, LogFile>();
    private Dictionary<string, List<Action<LogReadEventArgs>>> subscribers = [];
    private IDispatcherTimer? _timer;
    private bool isReadingFile = false;

    public PlayerLogReaderService() 
    {
        _timer = Dispatcher.GetForCurrentThread()?.CreateTimer();
        Debug.WriteLine(Dispatcher.GetForCurrentThread());
        if(_timer != null)
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }
        
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (_files.Count > 0)
        {
            CheckForChanges();
        }
    }

    private void onFileRead(LogReadEventArgs args)
    {
        foreach (Action<LogReadEventArgs> task in subscribers[args.FilePath])
        {
            task?.Invoke(args);
        }
    }

    public void AddLogFile(string filename, Action<LogReadEventArgs> action)
    {
        while (isReadingFile) { }
        if (!_files.ContainsKey(filename))
        {
            using(FileStream fs = new FileStream(filename,FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                
                _files.Add(filename, new LogFile { FilePath=filename, LastReadIndex=fs.Length });
            }
        }
        if (!subscribers.ContainsKey(filename)) {
            subscribers.Add(filename, []);
        }
        subscribers[filename].Add(action);
        
        
    }
    public void RemoveLogFile(string filename, Action<LogReadEventArgs> action)
    {
        while (isReadingFile) { }
        if (!subscribers.ContainsKey(filename)) { return; }
        subscribers[filename].Remove(action);
        if (subscribers[filename].Count == 0)
        {
            subscribers.Remove(filename);
            _files.Remove(filename);
        }
    }
    
    private void CheckForChanges()
    {
        isReadingFile = true;
        foreach (var file in _files.Keys)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if(fs.Length > _files[file].LastReadIndex)
                {
                    fs.Seek(_files[file].LastReadIndex, SeekOrigin.Begin);
                    List<string> lines = [];
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while (!sr.EndOfStream)
                        {
                            lines.Add(sr.ReadLine() ?? string.Empty);
                        }
                        if (_files.TryGetValue(file, out LogFile lf))
                        {
                            lf.LastReadIndex = fs.Position;
                            _files[file] = lf;
                        }
                        onFileRead(new LogReadEventArgs(file, lines));
                    }
                }
            }
        }
        isReadingFile = false;
    }

    
}

public struct LogFile
{
    public string FilePath { get; set; }
    public long LastReadIndex { get; set; }

}

public class LogReadEventArgs:EventArgs
{
    public string FilePath { get; set;}
    public List<string> Lines { get; set; }
    public LogReadEventArgs(string filePath, List<string> lines)
    {
        FilePath = filePath;
        Lines = lines;
    }
}
