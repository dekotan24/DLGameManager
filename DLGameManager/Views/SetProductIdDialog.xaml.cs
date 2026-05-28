using System.Windows;
using System.Windows.Input;
using DLGameManager.Services;

namespace DLGameManager.Views;

public partial class SetProductIdDialog : Window
{
    public string? ResultProductId { get; private set; }
    public string? ResultSource { get; private set; }

    public SetProductIdDialog(string? currentId = null)
    {
        InitializeComponent();
        if (!string.IsNullOrEmpty(currentId))
            IdTextBox.Text = currentId;
        IdTextBox.Focus();
    }

    private void OnOk_Click(object sender, RoutedEventArgs e) => TryAccept();
    private void OnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter) TryAccept();
        if (e.Key == System.Windows.Input.Key.Escape) DialogResult = false;
    }

    private void TryAccept()
    {
        var input = IdTextBox.Text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        var result = MetadataService.ExtractProductId(input);
        if (result != null)
        {
            ResultProductId = result.Id;
            ResultSource = result.Source;
            DialogResult = true;
            return;
        }

        // 直接IDとして解釈を試みる（ユーザーが RJ012345 や d_123456 をそのまま入力）
        if (input.StartsWith("RJ", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("VJ", StringComparison.OrdinalIgnoreCase) ||
            input.StartsWith("BJ", StringComparison.OrdinalIgnoreCase))
        {
            ResultProductId = input.ToUpper();
            ResultSource = "DLsite";
            DialogResult = true;
        }
        else if (input.StartsWith("d_", StringComparison.OrdinalIgnoreCase))
        {
            ResultProductId = input.ToLower();
            ResultSource = "FANZA";
            DialogResult = true;
        }
        else
        {
            PreviewText.Text = "無効なID形式です（RJ012345 / d_123456）";
            PreviewText.Foreground = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(0xf3, 0x8b, 0xa8));
        }
    }
}
