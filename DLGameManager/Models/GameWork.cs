using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DLGameManager.Models;

public class GameWork : ObservableObject
{
	private string _productId = "";

	private string _source = "";

	private string _title = "";

	private string _circleName = "";

	private string _folderPath = "";

	private string _exePath = "";

	private string _thumbnailPath = "";

	private string _thumbnailUrl = "";

	private string _tags = "";

	private DateTime? _registeredAt;

	private DateTime? _lastPlayedAt;

	private int _playCount;

	private int _rating;

	private string _memo = "";

	public int Id { get; set; }

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string ProductId
	{
		get
		{
			return _productId;
		}
		[MemberNotNull("_productId")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_productId, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("ProductId"));
				_productId = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ProductId"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string Source
	{
		get
		{
			return _source;
		}
		[MemberNotNull("_source")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_source, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("Source"));
				_source = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Source"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string Title
	{
		get
		{
			return _title;
		}
		[MemberNotNull("_title")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_title, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("Title"));
				_title = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Title"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string CircleName
	{
		get
		{
			return _circleName;
		}
		[MemberNotNull("_circleName")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_circleName, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("CircleName"));
				_circleName = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("CircleName"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string FolderPath
	{
		get
		{
			return _folderPath;
		}
		[MemberNotNull("_folderPath")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_folderPath, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("FolderPath"));
				_folderPath = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("FolderPath"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string ExePath
	{
		get
		{
			return _exePath;
		}
		[MemberNotNull("_exePath")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_exePath, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("ExePath"));
				_exePath = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ExePath"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string ThumbnailPath
	{
		get
		{
			return _thumbnailPath;
		}
		[MemberNotNull("_thumbnailPath")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_thumbnailPath, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("ThumbnailPath"));
				_thumbnailPath = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ThumbnailPath"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string ThumbnailUrl
	{
		get
		{
			return _thumbnailUrl;
		}
		[MemberNotNull("_thumbnailUrl")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_thumbnailUrl, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("ThumbnailUrl"));
				_thumbnailUrl = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ThumbnailUrl"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string Tags
	{
		get
		{
			return _tags;
		}
		[MemberNotNull("_tags")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_tags, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("Tags"));
				_tags = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Tags"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public DateTime? RegisteredAt
	{
		get
		{
			return _registeredAt;
		}
		set
		{
			if (!EqualityComparer<DateTime?>.Default.Equals(_registeredAt, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("RegisteredAt"));
				_registeredAt = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("RegisteredAt"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public DateTime? LastPlayedAt
	{
		get
		{
			return _lastPlayedAt;
		}
		set
		{
			if (!EqualityComparer<DateTime?>.Default.Equals(_lastPlayedAt, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("LastPlayedAt"));
				_lastPlayedAt = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("LastPlayedAt"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public int PlayCount
	{
		get
		{
			return _playCount;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_playCount, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("PlayCount"));
				_playCount = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("PlayCount"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public int Rating
	{
		get
		{
			return _rating;
		}
		set
		{
			if (!EqualityComparer<int>.Default.Equals(_rating, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("Rating"));
				_rating = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Rating"));
			}
		}
	}

	[GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator", "8.4.0.0")]
	[ExcludeFromCodeCoverage]
	public string Memo
	{
		get
		{
			return _memo;
		}
		[MemberNotNull("_memo")]
		set
		{
			if (!EqualityComparer<string>.Default.Equals(_memo, value))
			{
				OnPropertyChanging(new System.ComponentModel.PropertyChangingEventArgs("Memo"));
				_memo = value;
				OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Memo"));
			}
		}
	}
}
