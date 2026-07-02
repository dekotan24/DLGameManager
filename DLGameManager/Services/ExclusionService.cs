using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DLGameManager.Services;

/// <summary>
/// リストから手動削除した作品フォルダを、監視フォルダ/起動時スキャンによる自動再登録から
/// 除外するためのリスト。手動追加(フォルダ追加/一括追加/D&amp;D)された場合は除外を解除して再登録を許可する。
/// watch_folders.jsonと同じ流儀でJSONファイルに永続化する。(DLVoiceLibraryのexcluded_foldersパターンを移植)
/// </summary>
public class ExclusionService
{
	private readonly string _configPath;

	private readonly HashSet<string> _excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

	public ExclusionService()
	{
		_configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DLGameManager", "excluded_folders.json");
		Load();
	}

	public bool IsExcluded(string folderPath)
	{
		return _excluded.Contains(folderPath);
	}

	public void Add(string folderPath)
	{
		if (_excluded.Add(folderPath))
		{
			Save();
		}
	}

	public void Remove(string folderPath)
	{
		if (_excluded.Remove(folderPath))
		{
			Save();
		}
	}

	private void Load()
	{
		try
		{
			if (File.Exists(_configPath))
			{
				List<string>? list = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_configPath));
				if (list != null)
				{
					foreach (string item in list)
					{
						_excluded.Add(item);
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogService.Error("excluded_folders.json の読み込みに失敗", ex);
		}
	}

	private void Save()
	{
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
			File.WriteAllText(_configPath, JsonSerializer.Serialize(new List<string>(_excluded)));
		}
		catch (Exception ex)
		{
			LogService.Error("excluded_folders.json の保存に失敗", ex);
		}
	}
}
