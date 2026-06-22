<div align="center">

# 🎮 DL Game Manager

**DLsite / FANZA 対応のゲームライブラリ管理ツール for Windows**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-0078D6.svg)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

購入した同人ゲームをまとめて管理。フォルダを追加するだけで<br>
メタデータを自動取得し、サムネイル付きライブラリで一元管理できます。

[主な機能](#-主な機能) • [インストール](#-インストール) • [ビルド](#-ビルド) • [技術スタック](#-技術スタック)

</div>

---

## ✨ ハイライト

- 📂 **かんたん登録** — フォルダをドラッグ＆ドロップするだけ。親フォルダからの一括登録にも対応
- 🔍 **メタデータ自動取得** — 作品 ID (RJ/VJ/BJ/d\_) からタイトル・サークル名・サムネイル・タグを DLsite / FANZA から自動取得
- 👁️ **フォルダ監視** — 指定フォルダを監視し、新しい作品フォルダを自動検出・登録
- 🌐 **内蔵ブラウザ** — WebView2 ベースのブラウザで DLsite をアプリ内から直接閲覧
- 🏷️ **強力なフィルタ** — テキスト検索、ソース / サークル / タグ / ステータスでのフィルタ、複数ソート順
- 🎨 **Catppuccin テーマ** — 目に優しいダークテーマ (Catppuccin Mocha)

## 📋 主な機能

### 📥 登録

| 機能 | 説明 |
|------|------|
| ドラッグ＆ドロップ | ゲームフォルダをウィンドウにドロップして即登録 |
| フォルダ選択 | ダイアログからゲームフォルダを選択して追加 |
| 一括登録 | 親フォルダを指定して配下のゲームを一括追加 |
| フォルダ監視 | 指定フォルダを常時監視し、新規作品を自動検出 |
| 作品ID自動判定 | フォルダ名から DLsite (RJ/VJ/BJ) / FANZA (d\_) の作品 ID を自動推定 |

### 🔎 検索・整理

- **テキスト検索** — タイトル・サークル名で横断検索
- **ソースフィルタ** — DLsite / FANZA / 全て で切替
- **サークル・タグフィルタ** — 登録済みのサークル名・タグで絞り込み
- **ソート** — 最終起動日・登録日・タイトル・レーティングなど複数ソート順
- **プロパティ編集** — 作品の詳細情報やメモを手動編集

### 🌐 内蔵ブラウザ

- **DLsite 直接閲覧** — ライブラリとブラウザをタブ切替で行き来
- **作品ページへジャンプ** — ライブラリから作品の DLsite / FANZA ページを直接オープン
- **サークル検索** — ライブラリからサークル名で DLsite 検索

## 📦 インストール

### 動作要件

- Windows 10 以降
- [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
- [WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/) (Edge がインストール済みなら不要)

### ダウンロード

[Releases](https://github.com/dekotan24/DLGameManager/releases) ページからダウンロードしてください。

## 🔨 ビルド

```bash
dotnet build DLGameManager.sln
```

## 🛠️ 技術スタック

| カテゴリ | 技術 |
|---|---|
| フレームワーク | WPF (.NET 8.0) |
| アーキテクチャ | MVVM ([CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)) |
| データベース | SQLite ([Microsoft.Data.Sqlite](https://learn.microsoft.com/dotnet/standard/data/sqlite/)) |
| ブラウザ | [WebView2](https://developer.microsoft.com/microsoft-edge/webview2/) |
| 仮想化パネル | [VirtualizingWrapPanel](https://github.com/sbaeumlisberger/VirtualizingWrapPanel) |

## 📄 ライセンス

[MIT License](LICENSE)
