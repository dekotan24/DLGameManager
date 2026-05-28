using System.Windows;

namespace DLGameManager;

public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DispatcherUnhandledException += (s, ex) =>
        {
            System.IO.File.AppendAllText(
                System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                    "DLGameManager", "crash.log"),
                $"\n[{DateTime.Now:o}] {ex.Exception}\n");
            System.Windows.MessageBox.Show(
                $"エラーが発生しました:\n{ex.Exception.Message}\n\n{ex.Exception.StackTrace}",
                "DL Game Manager - Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            ex.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
        {
            System.IO.File.AppendAllText(
                System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                    "DLGameManager", "crash.log"),
                $"\n[{DateTime.Now:o}] FATAL: {ex.ExceptionObject}\n");
        };

        var window = new MainWindow();
        MainWindow = window;
        window.Show();
    }
}
