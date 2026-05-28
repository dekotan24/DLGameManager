using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DLGameManager.Models;
using DLGameManager.Services;
using DLGameManager.ViewModels;
using Microsoft.Web.WebView2.Core;

namespace DLGameManager;

public partial class MainWindow : Window
{
    private MainViewModel VM => (MainViewModel)DataContext;
    private bool _webViewInitialized;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            VM.BrowserNavigationRequested += url => Dispatcher.InvokeAsync(() => NavigateToUrl(url));
            UpdateTabAppearance();
        };
    }

    // --- WebView2 ---

    private async Task EnsureWebViewInitializedAsync()
    {
        if (_webViewInitialized) return;

        var userDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DLGameManager", "WebView2Data");

        var env = await CoreWebView2Environment.CreateAsync(
            browserExecutableFolder: null,
            userDataFolder: userDataFolder);

        await WebView.EnsureCoreWebView2Async(env);

        WebView.CoreWebView2.NavigationCompleted += (_, _) =>
        {
            VM.BrowserUrl = WebView.CoreWebView2.Source;
        };

        WebView.CoreWebView2.NewWindowRequested += (_, args) =>
        {
            args.Handled = true;
            WebView.CoreWebView2.Navigate(args.Uri);
        };

        _webViewInitialized = true;
    }

    private async Task NavigateToUrl(string url)
    {
        await EnsureWebViewInitializedAsync();

        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            url = "https://" + url;

        VM.BrowserUrl = url;
        VM.SwitchToBrowserTab();
        UpdateTabAppearance();
        WebView.CoreWebView2.Navigate(url);
    }

    // --- タブ切替 ---

    private void OnLibraryTab_Click(object sender, RoutedEventArgs e)
    {
        VM.SwitchToLibraryTab();
        UpdateTabAppearance();
    }

    private async void OnBrowserTab_Click(object sender, RoutedEventArgs e)
    {
        VM.SwitchToBrowserTab();
        UpdateTabAppearance();
        await EnsureWebViewInitializedAsync();
        if (WebView.CoreWebView2.Source == "about:blank" || string.IsNullOrEmpty(WebView.CoreWebView2.Source))
            WebView.CoreWebView2.Navigate(VM.BrowserUrl);
    }

    private void UpdateTabAppearance()
    {
        var active = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x89, 0xb4, 0xfa));
        var inactive = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x45, 0x45, 0x5a));
        LibraryTabButton.Background = VM.IsLibraryTabActive ? active : inactive;
        BrowserTabButton.Background = VM.IsBrowserTabActive ? active : inactive;
        LibraryTabButton.Foreground = VM.IsLibraryTabActive
            ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x1e, 0x1e, 0x2e))
            : new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xcd, 0xd6, 0xf4));
        BrowserTabButton.Foreground = VM.IsBrowserTabActive
            ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x1e, 0x1e, 0x2e))
            : new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xcd, 0xd6, 0xf4));
    }

    // --- ブラウザ操作 ---

    private async void OnNavigate_Click(object sender, RoutedEventArgs e)
        => await NavigateToUrl(VM.BrowserUrl);

    private async void OnUrlBar_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await NavigateToUrl(VM.BrowserUrl);
            e.Handled = true;
        }
    }

    private void OnBrowserBack_Click(object sender, RoutedEventArgs e)
    {
        if (WebView.CoreWebView2?.CanGoBack == true)
            WebView.CoreWebView2.GoBack();
    }

    private void OnBrowserForward_Click(object sender, RoutedEventArgs e)
    {
        if (WebView.CoreWebView2?.CanGoForward == true)
            WebView.CoreWebView2.GoForward();
    }

    private async void OnBrowserHome_Click(object sender, RoutedEventArgs e)
        => await NavigateToUrl("https://www.dlsite.com/maniax/");

    // --- 内蔵ブラウザコンテキストメニュー ---

    private void OnOpenInBuiltInBrowser_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement fe || fe.DataContext is not GameWork game) return;
        var url = MainViewModel.GetProductPageUrl(game);
        if (url != null) VM.RequestBrowserNavigation(url);
    }

    private void OnSearchCircleInBrowser_Click(object sender, RoutedEventArgs e)
    {
        string? circleName = null;
        if (sender is MenuItem mi && mi.Parent is ContextMenu cm
            && cm.PlacementTarget is TextBlock tb && tb.DataContext is GameWork game)
        {
            circleName = game.CircleName;
        }
        else if (sender is FrameworkElement fe && fe.DataContext is GameWork g)
        {
            circleName = g.CircleName;
        }
        if (!string.IsNullOrEmpty(circleName))
            VM.RequestBrowserNavigation(MainViewModel.GetCircleSearchUrl(circleName));
    }

    private void OnCircleNameFilter_Click(object sender, RoutedEventArgs e)
    {
        string? circleName = null;
        if (sender is MenuItem mi && mi.Parent is ContextMenu cm
            && cm.PlacementTarget is TextBlock tb && tb.DataContext is GameWork game)
        {
            circleName = game.CircleName;
        }
        if (!string.IsNullOrEmpty(circleName))
            VM.SelectedCircle = circleName;
    }

    // --- ドラッグ＆ドロップ ---

    private void OnDragOver(object sender, System.Windows.DragEventArgs e)
    {
        if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
        {
            var paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            e.Effects = paths.Any(Directory.Exists)
                ? System.Windows.DragDropEffects.Copy
                : System.Windows.DragDropEffects.None;
        }
        else
        {
            e.Effects = System.Windows.DragDropEffects.None;
        }
        e.Handled = true;
    }

    private void OnDrop(object sender, System.Windows.DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop)) return;

        var paths = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
        var folders = paths.Where(Directory.Exists).ToList();
        if (folders.Count == 0) return;

        VM.EnqueueFolders(folders);
    }

    // --- ボタン ---

    private void OnAddGame_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "作品フォルダを選択",
            UseDescriptionForTitle = true,
        };
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            VM.EnqueueFolders([dialog.SelectedPath]);
    }

    private void OnBatchAdd_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "作品フォルダが入っている親フォルダを選択",
            UseDescriptionForTitle = true,
        };
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            VM.EnqueueParentFolder(dialog.SelectedPath);
    }

    private void OnWatchFolders_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Views.WatchFoldersDialog(VM.WatchService) { Owner = this };
        if (dialog.ShowDialog() == true && dialog.ScanRequested)
        {
            var folders = VM.WatchService.ScanExistingFolders();
            VM.EnqueueFolders(folders);
        }
    }

    // --- スクロール量制御 ---

    private void OnListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is not System.Windows.Controls.ListBox listBox) return;
        var sv = FindScrollViewer(listBox);
        if (sv == null) return;
        sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta / 120.0 * 80);
        e.Handled = true;
    }

    private static System.Windows.Controls.ScrollViewer? FindScrollViewer(System.Windows.DependencyObject obj)
    {
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(obj, i);
            if (child is System.Windows.Controls.ScrollViewer sv) return sv;
            var result = FindScrollViewer(child);
            if (result != null) return result;
        }
        return null;
    }

    // --- カード操作 ---

    private void OnGameCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2 && sender is FrameworkElement fe && fe.DataContext is GameWork game)
            VM.LaunchGameCommand.Execute(game);
    }

    private void OnCircleName_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is GameWork game
            && !string.IsNullOrEmpty(game.CircleName))
        {
            VM.SelectedCircle = game.CircleName;
            e.Handled = true; // ダブルクリック起動を防止
        }
    }

    private void OnLaunchGame_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is GameWork game)
            VM.LaunchGameCommand.Execute(game);
    }

    private void OnOpenFolder_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is GameWork game)
            VM.OpenFolderCommand.Execute(game);
    }

    private void OnOpenProductPage_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement fe || fe.DataContext is not GameWork game) return;
        if (string.IsNullOrEmpty(game.ProductId) || string.IsNullOrEmpty(game.Source)) return;

        var url = game.Source switch
        {
            "DLsite" => $"https://www.dlsite.com/maniax/work/=/product_id/{game.ProductId}.html",
            "FANZA" => $"https://www.dmm.co.jp/dc/doujin/-/detail/=/cid={game.ProductId}/",
            _ => null,
        };
        if (url != null)
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    private void OnRefetchMetadata_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement fe || fe.DataContext is not GameWork game) return;
        if (string.IsNullOrEmpty(game.ProductId) || string.IsNullOrEmpty(game.Source))
        {
            System.Windows.MessageBox.Show(
                "作品IDが設定されていません。プロパティから設定してください。",
                "DL Game Manager", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            return;
        }
        VM.RefetchMetadata(game);
    }

    private void OnProperty_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement fe || fe.DataContext is not GameWork game) return;

        var dialog = new Views.PropertyDialog(game) { Owner = this };
        if (dialog.ShowDialog() == true)
        {
            VM.SaveGame(game);

            if (dialog.RefetchRequested)
                VM.RefetchMetadata(game);
        }
    }

    private void OnDeleteGame_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement fe || fe.DataContext is not GameWork game) return;

        var result = System.Windows.MessageBox.Show(
            $"「{game.Title}」をリストから削除しますか？\n（作品ファイルは削除されません）",
            "確認", MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
            VM.DeleteGameCommand.Execute(game);
    }
}
