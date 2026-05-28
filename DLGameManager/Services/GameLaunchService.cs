using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DLGameManager.Services;

public class GameLaunchService
{
	private static readonly string[] ExcludedPatterns = new string[13]
	{
		"unins", "setup", "install", "config", "update", "patch", "UnityCrashHandler", "CrashReporter", "dotnet", "vcredist",
		"dxsetup", "dxwebsetup", "DXSETUP"
	};

	public static List<string> FindExecutables(string folderPath)
	{
		if (!Directory.Exists(folderPath))
		{
			return new List<string>();
		}
		string[] files = Directory.GetFiles(folderPath, "*.exe", SearchOption.AllDirectories);
		string basePath = Path.GetFullPath(folderPath);
		return (from exe in files
			where !IsExcluded(Path.GetFileNameWithoutExtension(exe))
			select new
			{
				Path = exe,
				Depth = Path.GetFullPath(exe).Substring(basePath.Length).Count((char c) => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
			} into x
			orderby x.Depth, Path.GetFileName(x.Path)
			select x.Path).ToList();
	}

	public static string? FindBestExecutable(string folderPath)
	{
		List<string> list = FindExecutables(folderPath);
		return (list.Count > 0) ? list[0] : null;
	}

	public static Process? LaunchGame(string exePath)
	{
		if (!File.Exists(exePath))
		{
			return null;
		}
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			FileName = exePath,
			WorkingDirectory = (Path.GetDirectoryName(exePath) ?? ""),
			UseShellExecute = true
		};
		return Process.Start(startInfo);
	}

	private static bool IsExcluded(string fileNameWithoutExt)
	{
		return ExcludedPatterns.Any((string p) => fileNameWithoutExt.Contains(p, StringComparison.OrdinalIgnoreCase));
	}
}
