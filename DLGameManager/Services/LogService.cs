using System.IO;

namespace DLGameManager.Services;

public static class LogService
{
    private static readonly string _logPath;
    private static readonly object _lock = new();

    static LogService()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DLGameManager");
        Directory.CreateDirectory(dir);
        _logPath = Path.Combine(dir, "app.log");

        // 起動時に前回ログが1MB超えてたらローテート
        if (File.Exists(_logPath) && new FileInfo(_logPath).Length > 1_000_000)
        {
            var old = _logPath + ".old";
            if (File.Exists(old)) File.Delete(old);
            File.Move(_logPath, old);
        }
    }

    public static string LogPath => _logPath;

    public static void Info(string message) => Write("INFO", message);
    public static void Warn(string message) => Write("WARN", message);
    public static void Error(string message) => Write("ERROR", message);
    public static void Error(string message, Exception ex) => Write("ERROR", $"{message}: {ex.Message}");

    private static void Write(string level, string message)
    {
        var line = $"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {message}";
        lock (_lock)
        {
            try { File.AppendAllText(_logPath, line + Environment.NewLine); } catch { }
        }
    }
}
