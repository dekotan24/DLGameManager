using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Timers;

namespace DLGameManager.Services;

public class FolderWatchService : IDisposable
{
	private readonly List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

	private readonly ConcurrentDictionary<string, DateTime> _pendingFolders = new ConcurrentDictionary<string, DateTime>();

	private readonly System.Timers.Timer _debounceTimer;

	private readonly string _configPath;

	public List<string> WatchPaths { get; private set; } = new List<string>();

	public event Action<List<string>>? FoldersDetected;

	public FolderWatchService()
	{
		_configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DLGameManager", "watch_folders.json");
		_debounceTimer = new System.Timers.Timer(3000.0)
		{
			AutoReset = false
		};
		_debounceTimer.Elapsed += OnDebounceElapsed;
		LoadConfig();
	}

	public void AddWatchPath(string path)
	{
		if (!WatchPaths.Contains(path))
		{
			WatchPaths.Add(path);
			StartWatcher(path);
			SaveConfig();
		}
	}

	public void RemoveWatchPath(string path)
	{
		WatchPaths.Remove(path);
		FileSystemWatcher fileSystemWatcher = _watchers.FirstOrDefault((FileSystemWatcher w) => w.Path == path);
		if (fileSystemWatcher != null)
		{
			fileSystemWatcher.EnableRaisingEvents = false;
			fileSystemWatcher.Dispose();
			_watchers.Remove(fileSystemWatcher);
		}
		SaveConfig();
	}

	public List<string> ScanExistingFolders()
	{
		List<string> list = new List<string>();
		foreach (string watchPath in WatchPaths)
		{
			if (Directory.Exists(watchPath))
			{
				list.AddRange(Directory.GetDirectories(watchPath));
			}
		}
		return list;
	}

	public void StartAll()
	{
		StopAll();
		foreach (string watchPath in WatchPaths)
		{
			StartWatcher(watchPath);
		}
	}

	public void StopAll()
	{
		foreach (FileSystemWatcher watcher in _watchers)
		{
			watcher.EnableRaisingEvents = false;
			watcher.Dispose();
		}
		_watchers.Clear();
	}

	private void StartWatcher(string path)
	{
		if (Directory.Exists(path))
		{
			FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(path)
			{
				NotifyFilter = NotifyFilters.DirectoryName,
				IncludeSubdirectories = false,
				EnableRaisingEvents = true
			};
			fileSystemWatcher.Created += OnFolderCreated;
			fileSystemWatcher.Renamed += OnFolderRenamed;
			_watchers.Add(fileSystemWatcher);
		}
	}

	private void OnFolderCreated(object sender, FileSystemEventArgs e)
	{
		if (Directory.Exists(e.FullPath))
		{
			_pendingFolders[e.FullPath] = DateTime.Now;
			_debounceTimer.Stop();
			_debounceTimer.Start();
		}
	}

	private void OnFolderRenamed(object sender, RenamedEventArgs e)
	{
		if (Directory.Exists(e.FullPath))
		{
			_pendingFolders[e.FullPath] = DateTime.Now;
			_debounceTimer.Stop();
			_debounceTimer.Start();
		}
	}

	private void OnDebounceElapsed(object? sender, ElapsedEventArgs e)
	{
		List<string> list = _pendingFolders.Keys.ToList();
		_pendingFolders.Clear();
		if (list.Count > 0)
		{
			this.FoldersDetected?.Invoke(list);
		}
	}

	private void LoadConfig()
	{
		try
		{
			if (File.Exists(_configPath))
			{
				string json = File.ReadAllText(_configPath);
				WatchPaths = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
			}
		}
		catch
		{
			WatchPaths = new List<string>();
		}
	}

	private void SaveConfig()
	{
		try
		{
			string directoryName = Path.GetDirectoryName(_configPath);
			if (directoryName != null)
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(_configPath, JsonSerializer.Serialize(WatchPaths, new JsonSerializerOptions
			{
				WriteIndented = true
			}));
		}
		catch
		{
		}
	}

	public void Dispose()
	{
		_debounceTimer.Dispose();
		StopAll();
	}
}
