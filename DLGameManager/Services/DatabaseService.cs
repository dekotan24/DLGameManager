using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using DLGameManager.Models;
using Microsoft.Data.Sqlite;

namespace DLGameManager.Services;

public class DatabaseService
{
	private readonly string _dbPath;

	private readonly string _connectionString;

	public DatabaseService()
	{
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DLGameManager");
		Directory.CreateDirectory(text);
		_dbPath = Path.Combine(text, "games.db");
		_connectionString = "Data Source=" + _dbPath;
		InitializeDatabase();
	}

	private void InitializeDatabase()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		SqliteConnection val = new SqliteConnection(_connectionString);
		try
		{
			((DbConnection)(object)val).Open();
			SqliteCommand val2 = val.CreateCommand();
			try
			{
				((DbCommand)(object)val2).CommandText = "CREATE TABLE IF NOT EXISTS games (\n    id INTEGER PRIMARY KEY AUTOINCREMENT,\n    product_id TEXT NOT NULL,\n    source TEXT NOT NULL DEFAULT '',\n    title TEXT NOT NULL DEFAULT '',\n    circle_name TEXT NOT NULL DEFAULT '',\n    folder_path TEXT NOT NULL,\n    exe_path TEXT NOT NULL DEFAULT '',\n    thumbnail_path TEXT NOT NULL DEFAULT '',\n    thumbnail_url TEXT NOT NULL DEFAULT '',\n    tags TEXT NOT NULL DEFAULT '',\n    registered_at TEXT,\n    last_played_at TEXT,\n    play_count INTEGER NOT NULL DEFAULT 0,\n    rating INTEGER NOT NULL DEFAULT 0,\n    memo TEXT NOT NULL DEFAULT '',\n    UNIQUE(folder_path)\n)";
				((DbCommand)(object)val2).ExecuteNonQuery();
				string[] array = new string[3] { "ALTER TABLE games ADD COLUMN source TEXT NOT NULL DEFAULT ''", "ALTER TABLE games ADD COLUMN rating INTEGER NOT NULL DEFAULT 0", "ALTER TABLE games ADD COLUMN memo TEXT NOT NULL DEFAULT ''" };
				string[] array2 = array;
				foreach (string commandText in array2)
				{
					try
					{
						((DbCommand)(object)val2).CommandText = commandText;
						((DbCommand)(object)val2).ExecuteNonQuery();
					}
					catch
					{
					}
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public List<GameWork> GetAllGames()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		List<GameWork> list = new List<GameWork>();
		SqliteConnection val = new SqliteConnection(_connectionString);
		try
		{
			((DbConnection)(object)val).Open();
			SqliteCommand val2 = val.CreateCommand();
			try
			{
				((DbCommand)(object)val2).CommandText = "SELECT * FROM games ORDER BY id";
				SqliteDataReader val3 = val2.ExecuteReader();
				try
				{
					while (((DbDataReader)(object)val3).Read())
					{
						list.Add(ReadGame(val3));
					}
					return list;
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public GameWork? GetGameByFolderPath(string folderPath)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		SqliteConnection val = new SqliteConnection(_connectionString);
		try
		{
			((DbConnection)(object)val).Open();
			SqliteCommand val2 = val.CreateCommand();
			try
			{
				((DbCommand)(object)val2).CommandText = "SELECT * FROM games WHERE folder_path = @path";
				val2.Parameters.AddWithValue("@path", (object)folderPath);
				SqliteDataReader val3 = val2.ExecuteReader();
				try
				{
					return ((DbDataReader)(object)val3).Read() ? ReadGame(val3) : null;
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void InsertGame(GameWork game)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		SqliteConnection val = new SqliteConnection(_connectionString);
		try
		{
			((DbConnection)(object)val).Open();
			SqliteCommand val2 = val.CreateCommand();
			try
			{
				((DbCommand)(object)val2).CommandText = "INSERT INTO games (product_id, source, title, circle_name, folder_path, exe_path,\n                   thumbnail_path, thumbnail_url, tags, registered_at, last_played_at, play_count, rating, memo)\nVALUES (@product_id, @source, @title, @circle_name, @folder_path, @exe_path,\n        @thumbnail_path, @thumbnail_url, @tags, @registered_at, @last_played_at, @play_count, @rating, @memo)";
				BindGameParams(val2, game);
				((DbCommand)(object)val2).ExecuteNonQuery();
				((DbCommand)(object)val2).CommandText = "SELECT last_insert_rowid()";
				game.Id = Convert.ToInt32(((DbCommand)(object)val2).ExecuteScalar());
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void UpdateGame(GameWork game)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		SqliteConnection val = new SqliteConnection(_connectionString);
		try
		{
			((DbConnection)(object)val).Open();
			SqliteCommand val2 = val.CreateCommand();
			try
			{
				((DbCommand)(object)val2).CommandText = "UPDATE games SET\n    product_id = @product_id, source = @source, title = @title, circle_name = @circle_name,\n    folder_path = @folder_path, exe_path = @exe_path,\n    thumbnail_path = @thumbnail_path, thumbnail_url = @thumbnail_url,\n    tags = @tags, registered_at = @registered_at,\n    last_played_at = @last_played_at, play_count = @play_count,\n    rating = @rating, memo = @memo\nWHERE id = @id";
				val2.Parameters.AddWithValue("@id", (object)game.Id);
				BindGameParams(val2, game);
				((DbCommand)(object)val2).ExecuteNonQuery();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void DeleteGame(int id)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		SqliteConnection val = new SqliteConnection(_connectionString);
		try
		{
			((DbConnection)(object)val).Open();
			SqliteCommand val2 = val.CreateCommand();
			try
			{
				((DbCommand)(object)val2).CommandText = "DELETE FROM games WHERE id = @id";
				val2.Parameters.AddWithValue("@id", (object)id);
				((DbCommand)(object)val2).ExecuteNonQuery();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void BindGameParams(SqliteCommand cmd, GameWork game)
	{
		cmd.Parameters.AddWithValue("@product_id", (object)game.ProductId);
		cmd.Parameters.AddWithValue("@source", (object)game.Source);
		cmd.Parameters.AddWithValue("@title", (object)game.Title);
		cmd.Parameters.AddWithValue("@circle_name", (object)game.CircleName);
		cmd.Parameters.AddWithValue("@folder_path", (object)game.FolderPath);
		cmd.Parameters.AddWithValue("@exe_path", (object)game.ExePath);
		cmd.Parameters.AddWithValue("@thumbnail_path", (object)game.ThumbnailPath);
		cmd.Parameters.AddWithValue("@thumbnail_url", (object)game.ThumbnailUrl);
		cmd.Parameters.AddWithValue("@tags", (object)game.Tags);
		cmd.Parameters.AddWithValue("@registered_at", (object)(game.RegisteredAt.HasValue ? ((IConvertible)game.RegisteredAt.Value.ToString("o")) : ((IConvertible)DBNull.Value)));
		cmd.Parameters.AddWithValue("@last_played_at", (object)(game.LastPlayedAt.HasValue ? ((IConvertible)game.LastPlayedAt.Value.ToString("o")) : ((IConvertible)DBNull.Value)));
		cmd.Parameters.AddWithValue("@play_count", (object)game.PlayCount);
		cmd.Parameters.AddWithValue("@rating", (object)game.Rating);
		cmd.Parameters.AddWithValue("@memo", (object)game.Memo);
	}

	private static GameWork ReadGame(SqliteDataReader reader)
	{
		return new GameWork
		{
			Id = ((DbDataReader)(object)reader).GetInt32(((DbDataReader)(object)reader).GetOrdinal("id")),
			ProductId = ((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("product_id")),
			Source = ((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("source")),
			Title = ((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("title")),
			CircleName = ((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("circle_name")),
			FolderPath = ((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("folder_path")),
			ExePath = ((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("exe_path")),
			ThumbnailPath = ((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("thumbnail_path")),
			ThumbnailUrl = ((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("thumbnail_url")),
			Tags = ((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("tags")),
			RegisteredAt = (((DbDataReader)(object)reader).IsDBNull(((DbDataReader)(object)reader).GetOrdinal("registered_at")) ? ((DateTime?)null) : new DateTime?(DateTime.Parse(((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("registered_at"))))),
			LastPlayedAt = (((DbDataReader)(object)reader).IsDBNull(((DbDataReader)(object)reader).GetOrdinal("last_played_at")) ? ((DateTime?)null) : new DateTime?(DateTime.Parse(((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("last_played_at"))))),
			PlayCount = ((DbDataReader)(object)reader).GetInt32(((DbDataReader)(object)reader).GetOrdinal("play_count")),
			Rating = ((DbDataReader)(object)reader).GetInt32(((DbDataReader)(object)reader).GetOrdinal("rating")),
			Memo = ((DbDataReader)(object)reader).GetString(((DbDataReader)(object)reader).GetOrdinal("memo"))
		};
	}
}
