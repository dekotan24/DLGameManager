using System.Collections.ObjectModel;
using System.Windows;
using DLGameManager.Services;

namespace DLGameManager.Views;

public partial class WatchFoldersDialog : Window
{
    private readonly FolderWatchService _watchService;
    public ObservableCollection<string> Folders { get; }
    public bool ScanRequested { get; private set; }

    public WatchFoldersDialog(FolderWatchService watchService)
    {
        InitializeComponent();
        Services.DarkTitleBar.Apply(this);
        _watchService = watchService;
        Folders = new ObservableCollection<string>(watchService.WatchPaths);
        FolderListBox.ItemsSource = Folders;
    }

    private void OnAddFolder(object sender, RoutedEventArgs e)
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "監視するフォルダを選択",
            UseDescriptionForTitle = true,
        };

        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            var path = dialog.SelectedPath;
            if (!Folders.Contains(path))
            {
                Folders.Add(path);
                _watchService.AddWatchPath(path);
            }
        }
    }

    private void OnRemoveFolder(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.Tag is string path)
        {
            Folders.Remove(path);
            _watchService.RemoveWatchPath(path);
        }
    }

    private void OnScanExisting(object sender, RoutedEventArgs e)
    {
        ScanRequested = true;
        DialogResult = true;
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
