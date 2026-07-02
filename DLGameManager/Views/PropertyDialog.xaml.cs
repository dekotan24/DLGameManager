using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using DLGameManager.Models;
using DLGameManager.Services;
using Microsoft.Win32;

namespace DLGameManager.Views;

public partial class PropertyDialog : Window
{
	public GameWork Game { get; }

	public bool RefetchRequested { get; private set; }

	public bool FolderRenamed { get; private set; }

	public PropertyDialog(GameWork game)
	{
		InitializeComponent();
		Services.DarkTitleBar.Apply(this);
		Game = game;
		LoadFields();
		UpdateRenamePreview();
		TbProductId.TextChanged += delegate
		{
			UpdateRenamePreview();
		};
		TbTitle.TextChanged += delegate
		{
			UpdateRenamePreview();
		};
		TbCircle.TextChanged += delegate
		{
			UpdateRenamePreview();
		};
		CbSource.SelectionChanged += delegate
		{
			UpdateRenamePreview();
		};
	}

	private void LoadFields()
	{
		TbProductId.Text = Game.ProductId;
		TbTitle.Text = Game.Title;
		TbCircle.Text = Game.CircleName;
		TbTags.Text = Game.Tags;
		TbFolderPath.Text = Game.FolderPath;
		TbExePath.Text = Game.ExePath;
		TbThumbUrl.Text = Game.ThumbnailUrl;
		TbMemo.Text = Game.Memo;
		foreach (ComboBoxItem item in (IEnumerable)CbSource.Items)
		{
			if ((string)item.Content == Game.Source)
			{
				CbSource.SelectedItem = item;
				break;
			}
		}
		if (CbSource.SelectedItem == null)
		{
			CbSource.SelectedIndex = 2;
		}
		CbRating.SelectedIndex = Game.Rating;
		bool flag = Directory.Exists(Game.FolderPath);
		bool flag2 = !string.IsNullOrEmpty(Game.ExePath) && File.Exists(Game.ExePath);
		TbInfo.Text = $"登録日: {Game.RegisteredAt?.ToString("yyyy/MM/dd HH:mm") ?? "不明"}\n最終起動: {Game.LastPlayedAt?.ToString("yyyy/MM/dd HH:mm") ?? "未プレイ"}\n起動回数: {Game.PlayCount}\nフォルダ: {(flag ? "OK" : "⚠ 見つかりません")}\nexe: {(flag2 ? "OK" : (string.IsNullOrEmpty(Game.ExePath) ? "未設定" : "⚠ 見つかりません"))}\nサムネ: {(string.IsNullOrEmpty(Game.ThumbnailPath) ? "未取得" : "OK")}";
	}

	private string BuildNewFolderName()
	{
		string text = TbProductId.Text.Trim();
		string text2 = TbCircle.Text.Trim();
		string text3 = TbTitle.Text.Trim();
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		char[] invalid = Path.GetInvalidFileNameChars();
		string text4 = "[" + Clean(text) + "]";
		if (!string.IsNullOrEmpty(text2))
		{
			text4 = text4 + "[" + Clean(text2) + "]";
		}
		if (!string.IsNullOrEmpty(text3))
		{
			text4 = text4 + " " + Clean(text3);
		}
		return text4.Trim();
		string Clean(string s)
		{
			return new string(s.Select((char c) => Enumerable.Contains(invalid, c) ? '_' : c).ToArray());
		}
	}

	private void UpdateRenamePreview()
	{
		string text = BuildNewFolderName();
		if (string.IsNullOrEmpty(text))
		{
			TbRenamePreview.Text = "";
			return;
		}
		string fileName = Path.GetFileName(TbFolderPath.Text);
		if (fileName == text)
		{
			TbRenamePreview.Text = "（変更なし）";
			CbRenameFolder.IsChecked = false;
		}
		else
		{
			TbRenamePreview.Text = "→ " + text;
		}
	}

	private void ApplyFields()
	{
		Game.ProductId = TbProductId.Text.Trim();
		Game.Title = TbTitle.Text.Trim();
		Game.CircleName = TbCircle.Text.Trim();
		Game.Tags = TbTags.Text.Trim();
		Game.ThumbnailUrl = TbThumbUrl.Text.Trim();
		Game.Memo = TbMemo.Text;
		Game.Source = (CbSource.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
		Game.Rating = ((CbRating.SelectedIndex >= 0) ? CbRating.SelectedIndex : 0);
		if (CbRenameFolder.IsChecked == true)
		{
			string text = BuildNewFolderName();
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = TbFolderPath.Text.Trim();
				string directoryName = Path.GetDirectoryName(text2);
				if (directoryName != null && Directory.Exists(text2))
				{
					string text3 = Path.Combine(directoryName, text);
					if (text2 != text3)
					{
						try
						{
							if (!Directory.Exists(text3))
							{
								Directory.Move(text2, text3);
								Game.FolderPath = text3;
								if (!string.IsNullOrEmpty(Game.ExePath) && Game.ExePath.StartsWith(text2))
								{
									Game.ExePath = Game.ExePath.Replace(text2, text3);
								}
								TbFolderPath.Text = text3;
								TbExePath.Text = Game.ExePath;
								FolderRenamed = true;
							}
						}
						catch (Exception ex)
						{
							System.Windows.MessageBox.Show("フォルダのリネームに失敗しました:\n" + ex.Message, "DL Game Manager", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
						}
					}
				}
			}
		}
		Game.FolderPath = TbFolderPath.Text.Trim();
		Game.ExePath = TbExePath.Text.Trim();
	}

	private void OnSave(object sender, RoutedEventArgs e)
	{
		ApplyFields();
		base.DialogResult = true;
	}

	private void OnCancel(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
	}

	private void OnRefetch(object sender, RoutedEventArgs e)
	{
		ApplyFields();
		if (string.IsNullOrEmpty(Game.ProductId) || string.IsNullOrEmpty(Game.Source))
		{
			System.Windows.MessageBox.Show("作品IDとソースを設定してください。", "DL Game Manager", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
			return;
		}
		RefetchRequested = true;
		base.DialogResult = true;
	}

	private void OnChangeFolderPath(object sender, RoutedEventArgs e)
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
		{
			Description = "作品フォルダを選択",
			UseDescriptionForTitle = true,
			SelectedPath = Game.FolderPath
		};
		if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
		{
			TbFolderPath.Text = folderBrowserDialog.SelectedPath;
			UpdateRenamePreview();
		}
	}

	private void OnSelectExe(object sender, RoutedEventArgs e)
	{
		string text = TbFolderPath.Text;
		if (!Directory.Exists(text))
		{
			System.Windows.MessageBox.Show("フォルダが見つかりません。", "DL Game Manager", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
			return;
		}
		List<string> list = GameLaunchService.FindExecutables(text);
		if (list.Count == 0)
		{
			System.Windows.MessageBox.Show("実行ファイルが見つかりません。", "DL Game Manager", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Asterisk);
			return;
		}
		SelectExeDialog selectExeDialog = new SelectExeDialog(list, TbExePath.Text)
		{
			Owner = this
		};
		if (selectExeDialog.ShowDialog() == true && selectExeDialog.SelectedExePath != null)
		{
			TbExePath.Text = selectExeDialog.SelectedExePath;
		}
	}

	private void OnBrowseExe(object sender, RoutedEventArgs e)
	{
		Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
		{
			Filter = "実行ファイル (*.exe)|*.exe|すべてのファイル (*.*)|*.*",
			Title = "実行ファイルを選択"
		};
		if (openFileDialog.ShowDialog() == true)
		{
			TbExePath.Text = openFileDialog.FileName;
		}
	}

}
