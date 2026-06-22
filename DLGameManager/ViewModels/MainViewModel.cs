using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DLGameManager.Models;
using DLGameManager.Services;

namespace DLGameManager.ViewModels;

public class MainViewModel : ObservableObject
{
	private readonly DatabaseService _db = new DatabaseService();

	private readonly MetadataService _meta = new MetadataService();

	private readonly ConcurrentQueue<GameWork> _fetchQueue = new ConcurrentQueue<GameWork>();

	private readonly Dispatcher _dispatcher = ((DispatcherObject)System.Windows.Application.Current).Dispatcher;

	private int _isFetching;

	private string _filterText = "";

	private string _selectedSort = "最終起動日（新しい順）";

	private string _selectedSource = "全て";

	private string _selectedCircle = "全て";

	private string _selectedTag = "全て";

	private string _selectedStatus = "全て";

	private bool _isLibraryTabActive = true;

	private bool _isBrowserTabActive;

	private string _browserUrl = "https://www.dlsite.com/maniax/";

	private bool _isLoading;

	private string _loadingMessage = "";

	private int _fetchTotal;

	private int _fetchDone;

	private GameWork? _selectedGame;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
	private RelayCommand<GameWork?>? launchGameCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
	private RelayCommand<GameWork?>? changeExePathCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
	private RelayCommand<GameWork?>? deleteGameCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
	private RelayCommand<GameWork?>? openFolderCommand;

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
	private RelayCommand? clearFiltersCommand;

	public FolderWatchService WatchService { get; } = new FolderWatchService();

	public ObservableCollection<GameWork> Games { get; } = new ObservableCollection<GameWork>();

	public ICollectionView GamesView { get; }

	public string StatusText => $"{((IEnumerable)GamesView).Cast<object>().Count()} / {Games.Count} 作品";

	public string[] SortOptions { get; } = new string[8] { "最終起動日（新しい順）", "最終起動日（古い順）", "タイトル（A→Z）", "タイトル（Z→A）", "登録日（新しい順）", "登録日（古い順）", "起動回数（多い順）", "評価（高い順）" };

	public string[] SourceOptions { get; } = new string[3] { "全て", "DLsite", "FANZA" };

	public string[] StatusOptions { get; } = new string[10] { "全て", "プレイ済み", "未プレイ", "フォルダなし", "作品IDあり", "作品IDなし", "サムネ未取得", "評価あり", "未評価", "メモあり" };

	public ObservableCollection<string> CircleOptions { get; } = new ObservableCollection<string> { "全て" };

	public ObservableCollection<string> TagOptions { get; } = new ObservableCollection<string> { "全て" };

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string FilterText
	{
		get
		{
			return _filterText;
		}
		[MemberNotNull("_filterText")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_filterText, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("FilterText"));
				_filterText = value;
				OnFilterTextChanged(value);
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("FilterText"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedSort
	{
		get
		{
			return _selectedSort;
		}
		[MemberNotNull("_selectedSort")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedSort, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("SelectedSort"));
				_selectedSort = value;
				OnSelectedSortChanged(value);
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedSort"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedSource
	{
		get
		{
			return _selectedSource;
		}
		[MemberNotNull("_selectedSource")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedSource, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("SelectedSource"));
				_selectedSource = value;
				OnSelectedSourceChanged(value);
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedSource"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedCircle
	{
		get
		{
			return _selectedCircle;
		}
		[MemberNotNull("_selectedCircle")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedCircle, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("SelectedCircle"));
				_selectedCircle = value;
				OnSelectedCircleChanged(value);
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedCircle"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedTag
	{
		get
		{
			return _selectedTag;
		}
		[MemberNotNull("_selectedTag")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedTag, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("SelectedTag"));
				_selectedTag = value;
				OnSelectedTagChanged(value);
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedTag"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string SelectedStatus
	{
		get
		{
			return _selectedStatus;
		}
		[MemberNotNull("_selectedStatus")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_selectedStatus, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("SelectedStatus"));
				_selectedStatus = value;
				OnSelectedStatusChanged(value);
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedStatus"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsLibraryTabActive
	{
		get
		{
			return _isLibraryTabActive;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLibraryTabActive, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("IsLibraryTabActive"));
				_isLibraryTabActive = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsLibraryTabActive"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsBrowserTabActive
	{
		get
		{
			return _isBrowserTabActive;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isBrowserTabActive, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("IsBrowserTabActive"));
				_isBrowserTabActive = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsBrowserTabActive"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string BrowserUrl
	{
		get
		{
			return _browserUrl;
		}
		[MemberNotNull("_browserUrl")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_browserUrl, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("BrowserUrl"));
				_browserUrl = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("BrowserUrl"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public bool IsLoading
	{
		get
		{
			return _isLoading;
		}
		set
		{
			if (!EqualityComparer<bool>.Default.Equals(_isLoading, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("IsLoading"));
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("StatusText"));
				_isLoading = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsLoading"));
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("StatusText"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string LoadingMessage
	{
		get
		{
			return _loadingMessage;
		}
		[MemberNotNull("_loadingMessage")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_loadingMessage, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("LoadingMessage"));
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("StatusText"));
				_loadingMessage = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("LoadingMessage"));
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("StatusText"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public int FetchTotal
	{
		get
		{
			return _fetchTotal;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_fetchTotal, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("FetchTotal"));
				_fetchTotal = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("FetchTotal"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public int FetchDone
	{
		get
		{
			return _fetchDone;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_fetchDone, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("FetchDone"));
				_fetchDone = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("FetchDone"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public GameWork? SelectedGame
	{
		get
		{
			return _selectedGame;
		}
		set
		{
			if (!EqualityComparer<GameWork>.Default.Equals(_selectedGame, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("SelectedGame"));
				_selectedGame = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SelectedGame"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<GameWork?> LaunchGameCommand => (IRelayCommand<GameWork?>)(object)(launchGameCommand ?? (launchGameCommand = new RelayCommand<GameWork>((Action<GameWork>)LaunchGame)));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<GameWork?> ChangeExePathCommand => (IRelayCommand<GameWork?>)(object)(changeExePathCommand ?? (changeExePathCommand = new RelayCommand<GameWork>((Action<GameWork>)ChangeExePath)));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<GameWork?> DeleteGameCommand => (IRelayCommand<GameWork?>)(object)(deleteGameCommand ?? (deleteGameCommand = new RelayCommand<GameWork>((Action<GameWork>)DeleteGame)));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand<GameWork?> OpenFolderCommand => (IRelayCommand<GameWork?>)(object)(openFolderCommand ?? (openFolderCommand = new RelayCommand<GameWork>((Action<GameWork>)OpenFolder)));

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public IRelayCommand ClearFiltersCommand
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			//IL_0023: Expected O, but got Unknown
			RelayCommand obj = clearFiltersCommand;
			if (obj == null)
			{
				RelayCommand val = new RelayCommand((Action)ClearFilters);
				RelayCommand val2 = val;
				clearFiltersCommand = val;
				obj = val2;
			}
			return (IRelayCommand)(object)obj;
		}
	}

	public event Action<string>? BrowserNavigationRequested;

	public MainViewModel()
	{
		GamesView = CollectionViewSource.GetDefaultView(Games);
		GamesView.Filter = FilterGame;
		LoadGames();
		WatchService.FoldersDetected += delegate(List<string> folders)
		{
			_dispatcher.Invoke((Action)delegate
			{
				EnqueueFolders(folders);
			});
		};
		WatchService.StartAll();
		Task.Run(() =>
		{
			var missedFolders = WatchService.ScanExistingFolders();
			if (missedFolders.Count > 0)
			{
				_dispatcher.Invoke(() => EnqueueFolders(missedFolders));
			}
		});
	}

	private void RefreshView()
	{
		GamesView.Refresh();
		OnPropertyChanged("StatusText");
	}

	private bool FilterGame(object obj)
	{
		if (!(obj is GameWork gameWork))
		{
			return false;
		}
		if (!string.IsNullOrWhiteSpace(FilterText))
		{
			string value = FilterText.Trim();
			if (!gameWork.Title.Contains(value, StringComparison.OrdinalIgnoreCase) && !gameWork.CircleName.Contains(value, StringComparison.OrdinalIgnoreCase) && !gameWork.ProductId.Contains(value, StringComparison.OrdinalIgnoreCase) && !gameWork.Tags.Contains(value, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}
		if (SelectedSource != "全て" && gameWork.Source != SelectedSource)
		{
			return false;
		}
		if (SelectedCircle != "全て" && !string.IsNullOrEmpty(SelectedCircle) && !gameWork.CircleName.Contains(SelectedCircle, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		if (SelectedTag != "全て" && !string.IsNullOrEmpty(SelectedTag) && !gameWork.Tags.Contains(SelectedTag, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		switch (SelectedStatus)
		{
		case "プレイ済み":
			if (gameWork.LastPlayedAt.HasValue)
			{
				break;
			}
			return false;
		case "未プレイ":
			if (!gameWork.LastPlayedAt.HasValue)
			{
				break;
			}
			return false;
		case "フォルダなし":
			if (!Directory.Exists(gameWork.FolderPath))
			{
				break;
			}
			return false;
		case "作品IDあり":
			if (!string.IsNullOrEmpty(gameWork.ProductId))
			{
				break;
			}
			return false;
		case "作品IDなし":
			if (string.IsNullOrEmpty(gameWork.ProductId))
			{
				break;
			}
			return false;
		case "サムネ未取得":
			if (string.IsNullOrEmpty(gameWork.ThumbnailPath))
			{
				break;
			}
			return false;
		case "評価あり":
			if (gameWork.Rating != 0)
			{
				break;
			}
			return false;
		case "未評価":
			if (gameWork.Rating == 0)
			{
				break;
			}
			return false;
		case "メモあり":
			if (!string.IsNullOrEmpty(gameWork.Memo))
			{
				break;
			}
			return false;
		}
		return true;
	}

	private void ApplySort()
	{
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		((Collection<SortDescription>)(object)GamesView.SortDescriptions).Clear();
		switch (SelectedSort)
		{
		case "最終起動日（新しい順）":
			((Collection<SortDescription>)(object)GamesView.SortDescriptions).Add(new SortDescription("LastPlayedAt", ListSortDirection.Descending));
			break;
		case "最終起動日（古い順）":
			((Collection<SortDescription>)(object)GamesView.SortDescriptions).Add(new SortDescription("LastPlayedAt", ListSortDirection.Ascending));
			break;
		case "タイトル（A→Z）":
			((Collection<SortDescription>)(object)GamesView.SortDescriptions).Add(new SortDescription("Title", ListSortDirection.Ascending));
			break;
		case "タイトル（Z→A）":
			((Collection<SortDescription>)(object)GamesView.SortDescriptions).Add(new SortDescription("Title", ListSortDirection.Descending));
			break;
		case "登録日（新しい順）":
			((Collection<SortDescription>)(object)GamesView.SortDescriptions).Add(new SortDescription("RegisteredAt", ListSortDirection.Descending));
			break;
		case "登録日（古い順）":
			((Collection<SortDescription>)(object)GamesView.SortDescriptions).Add(new SortDescription("RegisteredAt", ListSortDirection.Ascending));
			break;
		case "起動回数（多い順）":
			((Collection<SortDescription>)(object)GamesView.SortDescriptions).Add(new SortDescription("PlayCount", ListSortDirection.Descending));
			break;
		case "評価（高い順）":
			((Collection<SortDescription>)(object)GamesView.SortDescriptions).Add(new SortDescription("Rating", ListSortDirection.Descending));
			break;
		}
		OnPropertyChanged("StatusText");
	}

	private void RebuildFilterOptions()
	{
		List<string> list = (from c in (from g in Games
				select g.CircleName into c
				where !string.IsNullOrEmpty(c)
				select c).Distinct()
			orderby c
			select c).ToList();
		CircleOptions.Clear();
		CircleOptions.Add("全て");
		foreach (string item in list)
		{
			CircleOptions.Add(item);
		}
		List<string> list2 = (from t in (from t in Games.SelectMany((GameWork g) => g.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
				select t.Trim() into t
				where !string.IsNullOrEmpty(t)
				select t).Distinct()
			orderby t
			select t).ToList();
		TagOptions.Clear();
		TagOptions.Add("全て");
		foreach (string item2 in list2)
		{
			TagOptions.Add(item2);
		}
	}

	private void LoadGames()
	{
		LogService.Info("=== App starting, loading games ===");
		Games.Clear();
		foreach (GameWork allGame in _db.GetAllGames())
		{
			Games.Add(allGame);
			if (!string.IsNullOrEmpty(allGame.ProductId) && string.IsNullOrEmpty(allGame.Source))
			{
				ProductIdResult productIdResult = MetadataService.ExtractProductId(allGame.ProductId);
				if (productIdResult != null)
				{
					allGame.Source = productIdResult.Source;
					_db.UpdateGame(allGame);
				}
			}
			if (!string.IsNullOrEmpty(allGame.ProductId) && !string.IsNullOrEmpty(allGame.Source) && string.IsNullOrEmpty(allGame.ThumbnailPath))
			{
				_fetchQueue.Enqueue(allGame);
			}
		}
		LogService.Info($"Loaded {Games.Count} games, {_fetchQueue.Count} pending fetch");
		RebuildFilterOptions();
		ApplySort();
		if (!_fetchQueue.IsEmpty)
		{
			StartFetchWorker();
		}
	}

	public void EnqueueFolders(IEnumerable<string> folderPaths)
	{
		foreach (string folderPath in folderPaths)
		{
			if (Directory.Exists(folderPath) && _db.GetGameByFolderPath(folderPath) == null)
			{
				string fileName = Path.GetFileName(folderPath);
				ProductIdResult productIdResult = MetadataService.ExtractProductId(fileName);
				GameWork gameWork = new GameWork
				{
					ProductId = (productIdResult?.Id ?? ""),
					Source = (productIdResult?.Source ?? ""),
					Title = (fileName ?? "不明"),
					FolderPath = folderPath,
					RegisteredAt = DateTime.Now
				};
				string text = GameLaunchService.FindBestExecutable(folderPath);
				if (text != null)
				{
					gameWork.ExePath = text;
				}
				_db.InsertGame(gameWork);
				Games.Add(gameWork);
				if (productIdResult != null)
				{
					_fetchQueue.Enqueue(gameWork);
				}
			}
		}
		RebuildFilterOptions();
		OnPropertyChanged("StatusText");
		GamesView.Refresh();
		StartFetchWorker();
	}

	public void EnqueueParentFolder(string parentPath)
	{
		if (Directory.Exists(parentPath))
		{
			EnqueueFolders(Directory.GetDirectories(parentPath));
		}
	}

	private void StartFetchWorker()
	{
		if (Interlocked.CompareExchange(ref _isFetching, 1, 0) != 0)
		{
			return;
		}
		Task.Run(async delegate
		{
			_dispatcher.Invoke((Action)delegate
			{
				FetchTotal = _fetchQueue.Count;
				FetchDone = 0;
			});
			try
			{
				GameWork game;
				while (_fetchQueue.TryDequeue(out game))
				{
					_dispatcher.Invoke((Action)delegate
					{
						IsLoading = true;
						FetchTotal = FetchDone + _fetchQueue.Count + 1;
						LoadingMessage = $"取得中: [{game.Source}] {game.ProductId} ({FetchDone + 1}/{FetchTotal})";
					});
					GameWork info = await _meta.FetchWorkInfoAsync(game.ProductId, game.Source);
					if (info != null)
					{
						_dispatcher.Invoke((Action)delegate
						{
							game.Title = info.Title;
							game.CircleName = info.CircleName;
							game.ThumbnailUrl = info.ThumbnailUrl;
							game.Tags = info.Tags;
						});
						if (!string.IsNullOrEmpty(info.ThumbnailUrl))
						{
							string thumbPath = await _meta.DownloadThumbnailAsync(game.ProductId, info.ThumbnailUrl);
							if (thumbPath != null)
							{
								_dispatcher.Invoke<string>((Func<string>)(() => game.ThumbnailPath = thumbPath));
							}
						}
					}
					_db.UpdateGame(game);
					_dispatcher.Invoke((Action)delegate
					{
						FetchDone++;
						RebuildFilterOptions();
						GamesView.Refresh();
						OnPropertyChanged("StatusText");
					});
					await Task.Delay(2000);
				}
			}
			finally
			{
				Interlocked.Exchange(ref _isFetching, 0);
				_dispatcher.Invoke((Action)delegate
				{
					IsLoading = false;
					LoadingMessage = "";
					RebuildFilterOptions();
					OnPropertyChanged("StatusText");
				});
			}
		});
	}

	public void SaveGame(GameWork game)
	{
		_db.UpdateGame(game);
		RebuildFilterOptions();
		GamesView.Refresh();
		OnPropertyChanged("StatusText");
	}

	public void SetProductIdAndFetch(GameWork game, string productId, string source)
	{
		game.ProductId = productId;
		game.Source = source;
		game.ThumbnailPath = "";
		_db.UpdateGame(game);
		_fetchQueue.Enqueue(game);
		StartFetchWorker();
	}

	public void RefetchMetadata(GameWork game)
	{
		game.ThumbnailPath = "";
		_db.UpdateGame(game);
		_fetchQueue.Enqueue(game);
		StartFetchWorker();
	}

	private void LaunchGame(GameWork? game)
	{
		if (game == null || string.IsNullOrEmpty(game.ExePath))
		{
			return;
		}
		if (!File.Exists(game.ExePath))
		{
			System.Windows.MessageBox.Show("実行ファイルが見つかりません:\n" + game.ExePath + "\n\nフォルダが移動・削除された可能性があります。", "DL Game Manager", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
			return;
		}
		Process process = GameLaunchService.LaunchGame(game.ExePath);
		if (process != null)
		{
			game.LastPlayedAt = DateTime.Now;
			game.PlayCount++;
			_db.UpdateGame(game);
			OnPropertyChanged("StatusText");
			GamesView.Refresh();
		}
	}

	private void ChangeExePath(GameWork? game)
	{
	}

	public void UpdateGameExePath(GameWork game, string newExePath)
	{
		game.ExePath = newExePath;
		_db.UpdateGame(game);
	}

	private void DeleteGame(GameWork? game)
	{
		if (game != null)
		{
			_db.DeleteGame(game.Id);
			Games.Remove(game);
			RebuildFilterOptions();
			OnPropertyChanged("StatusText");
		}
	}

	private void OpenFolder(GameWork? game)
	{
		if (game != null)
		{
			if (!Directory.Exists(game.FolderPath))
			{
				System.Windows.MessageBox.Show("フォルダが見つかりません:\n" + game.FolderPath, "DL Game Manager", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
			}
			else
			{
				Process.Start("explorer.exe", game.FolderPath);
			}
		}
	}

	private void ClearFilters()
	{
		FilterText = "";
		SelectedSource = "全て";
		SelectedCircle = "全て";
		SelectedTag = "全て";
		SelectedStatus = "全て";
	}

	public void SwitchToLibraryTab()
	{
		IsLibraryTabActive = true;
		IsBrowserTabActive = false;
	}

	public void SwitchToBrowserTab()
	{
		IsLibraryTabActive = false;
		IsBrowserTabActive = true;
	}

	public void RequestBrowserNavigation(string url)
	{
		BrowserUrl = url;
		SwitchToBrowserTab();
		this.BrowserNavigationRequested?.Invoke(url);
	}

	public static string? GetProductPageUrl(GameWork game)
	{
		if (string.IsNullOrEmpty(game.ProductId) || string.IsNullOrEmpty(game.Source))
		{
			return null;
		}
		string source = game.Source;
		if (1 == 0)
		{
		}
		string result = ((source == "DLsite") ? ("https://www.dlsite.com/maniax/work/=/product_id/" + game.ProductId + ".html") : ((!(source == "FANZA")) ? null : ("https://www.dmm.co.jp/dc/doujin/-/detail/=/cid=" + game.ProductId + "/")));
		if (1 == 0)
		{
		}
		return result;
	}

	public static string GetCircleSearchUrl(string circleName)
	{
		string text = Uri.EscapeDataString(circleName);
		return "https://www.dlsite.com/maniax/fsr/=/keyword/" + text + "/";
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	private void OnFilterTextChanged(string value)
	{
		RefreshView();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	private void OnSelectedSortChanged(string value)
	{
		ApplySort();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	private void OnSelectedSourceChanged(string value)
	{
		RefreshView();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	private void OnSelectedCircleChanged(string value)
	{
		RefreshView();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	private void OnSelectedTagChanged(string value)
	{
		RefreshView();
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	private void OnSelectedStatusChanged(string value)
	{
		RefreshView();
	}
}
