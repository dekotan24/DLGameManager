using System.Windows;

namespace DLGameManager.Views;

public partial class SelectExeDialog : Window
{
    public string? SelectedExePath { get; private set; }

    public SelectExeDialog(List<string> exePaths, string? currentExe)
    {
        InitializeComponent();
        ExeListBox.ItemsSource = exePaths;

        if (currentExe != null && exePaths.Contains(currentExe))
            ExeListBox.SelectedItem = currentExe;
        else if (exePaths.Count > 0)
            ExeListBox.SelectedIndex = 0;
    }

    private void OnSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ExeListBox.SelectedItem is string path)
        {
            SelectedExePath = path;
            DialogResult = true;
        }
    }

    private void OnBrowse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "実行ファイル (*.exe)|*.exe|すべてのファイル (*.*)|*.*",
            Title = "実行ファイルを選択",
        };

        if (dialog.ShowDialog() == true)
        {
            SelectedExePath = dialog.FileName;
            DialogResult = true;
        }
    }

    private void OnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void OnListBox_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (ExeListBox.SelectedItem is string path)
        {
            SelectedExePath = path;
            DialogResult = true;
        }
    }
}
