using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DLGameManager.Models;

namespace DLGameManager.Services;

public class MetadataService
{
	private static readonly HttpClient _client;

	private readonly string _thumbnailDir;

	static MetadataService()
	{
		_client = new HttpClient();
		_client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
	}

	public MetadataService()
	{
		_thumbnailDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DLGameManager", "thumbnails");
		Directory.CreateDirectory(_thumbnailDir);
	}

	public static ProductIdResult? ExtractProductId(string folderName)
	{
		Match match = Regex.Match(folderName, "(RJ|VJ|BJ|RG|RE)(\\d{6,8})", RegexOptions.IgnoreCase);
		if (match.Success)
		{
			return new ProductIdResult(match.Groups[1].Value.ToUpper() + match.Groups[2].Value, "DLsite");
		}
		Match match2 = Regex.Match(folderName, "d_(\\d+)", RegexOptions.IgnoreCase);
		if (match2.Success)
		{
			return new ProductIdResult("d_" + match2.Groups[1].Value, "FANZA");
		}
		return null;
	}

	public async Task<GameWork?> FetchWorkInfoAsync(string productId, string source)
	{
		if (1 == 0)
		{
		}
		GameWork result = ((source == "DLsite") ? (await FetchDLsiteInfoAsync(productId)) : ((!(source == "FANZA")) ? null : (await FetchFanzaInfoAsync(productId))));
		if (1 == 0)
		{
		}
		return result;
	}

	private async Task<GameWork?> FetchDLsiteInfoAsync(string productId)
	{
		try
		{
			LogService.Info("[DLsite] Fetching " + productId);
			string url = "https://www.dlsite.com/maniax/product/info/ajax?product_id=" + productId;
			HttpResponseMessage response = await _client.GetAsync(url);
			if (!response.IsSuccessStatusCode)
			{
				LogService.Warn($"[DLsite] {productId} API returned {response.StatusCode}");
				return null;
			}
			using JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
			if (!doc.RootElement.TryGetProperty(productId, out var work))
			{
				LogService.Warn("[DLsite] " + productId + " not found in API response");
				return null;
			}
			GameWork game = new GameWork
			{
				ProductId = productId,
				Source = "DLsite",
				Title = (GetJsonString(work, "work_name") ?? productId)
			};
			string imgUrl = GetJsonString(work, "work_image");
			if (!string.IsNullOrEmpty(imgUrl))
			{
				if (imgUrl.StartsWith("//"))
				{
					imgUrl = "https:" + imgUrl;
				}
				game.ThumbnailUrl = imgUrl;
			}
			await FetchDLsiteHtmlDetailsAsync(productId, game);
			LogService.Info($"[DLsite] {productId} -> title={game.Title}, circle={game.CircleName}, thumb={!string.IsNullOrEmpty(game.ThumbnailUrl)}, tags={game.Tags.Split(',').Length}");
			return game;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			LogService.Error("[DLsite] " + productId + " fetch failed", ex2);
			return null;
		}
	}

	private async Task FetchDLsiteHtmlDetailsAsync(string productId, GameWork game)
	{
		try
		{
			string url = "https://www.dlsite.com/maniax/work/=/product_id/" + productId + ".html";
			string html = await _client.GetStringAsync(url);
			Match makerMatch = Regex.Match(html, "data-maker-name=\"([^\"]+)\"");
			if (makerMatch.Success)
			{
				game.CircleName = WebUtility.HtmlDecode(makerMatch.Groups[1].Value);
			}
			Match genreSection = Regex.Match(html, "class=\"main_genre\"[^>]*>(.*?)</div>", RegexOptions.Singleline);
			if (!genreSection.Success)
			{
				return;
			}
			MatchCollection tagMatches = Regex.Matches(genreSection.Groups[1].Value, ">([^<]+)</a>");
			List<string> tags = new List<string>();
			foreach (Match m in tagMatches)
			{
				string tag = WebUtility.HtmlDecode(m.Groups[1].Value).Trim();
				if (!string.IsNullOrEmpty(tag))
				{
					tags.Add(tag);
				}
			}
			if (tags.Count > 0)
			{
				game.Tags = string.Join(",", tags);
			}
		}
		catch
		{
		}
	}

	private async Task<GameWork?> FetchFanzaInfoAsync(string productId)
	{
		LogService.Info("[FANZA] Fetching " + productId);
		try
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://www.dmm.co.jp/dc/doujin/-/detail/=/cid=" + productId + "/")
			{
				Headers = { { "Cookie", "age_check_done=1" } }
			};
			HttpResponseMessage response = await _client.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				LogService.Warn($"[FANZA] {productId} returned {response.StatusCode}");
				return null;
			}
			string html = await response.Content.ReadAsStringAsync();
			GameWork game = new GameWork
			{
				ProductId = productId,
				Source = "FANZA"
			};
			Match titleMatch = Regex.Match(html, "<title>(.+?)｜FANZA");
			if (titleMatch.Success)
			{
				string raw = WebUtility.HtmlDecode(titleMatch.Groups[1].Value).Trim();
				Match circleMatch = Regex.Match(raw, "^(.+)\\(([^)]+)\\)$");
				if (circleMatch.Success)
				{
					game.Title = circleMatch.Groups[1].Value.Trim();
					game.CircleName = circleMatch.Groups[2].Value.Trim();
				}
				else
				{
					game.Title = raw;
				}
			}
			if (string.IsNullOrEmpty(game.Title))
			{
				game.Title = productId;
			}
			Match ogImage = Regex.Match(html, "og:image\"\\s+content=\"([^\"]+)\"");
			if (ogImage.Success)
			{
				game.ThumbnailUrl = ogImage.Groups[1].Value;
			}
			MatchCollection genreMatches = Regex.Matches(html, "article=keyword/id=(\\d+)/[^\"]*\">([^<]+)</a>");
			if (genreMatches.Count > 0)
			{
				List<string> tags = new List<string>();
				foreach (Match m in genreMatches)
				{
					string tag = WebUtility.HtmlDecode(m.Groups[2].Value).Trim();
					if (!string.IsNullOrEmpty(tag) && !tags.Contains(tag))
					{
						tags.Add(tag);
					}
				}
				if (tags.Count > 0)
				{
					game.Tags = string.Join(",", tags);
				}
			}
			LogService.Info($"[FANZA] {productId} -> title={game.Title}, circle={game.CircleName}, thumb={!string.IsNullOrEmpty(game.ThumbnailUrl)}, tags={game.Tags.Split(',').Length}");
			return game;
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			LogService.Error("[FANZA] " + productId + " fetch failed", ex2);
			return null;
		}
	}

	public async Task<string?> DownloadThumbnailAsync(string productId, string imageUrl)
	{
		try
		{
			string ext = Path.GetExtension(new Uri(imageUrl).AbsolutePath);
			if (string.IsNullOrEmpty(ext))
			{
				ext = ".jpg";
			}
			string localPath = Path.Combine(_thumbnailDir, productId + ext);
			if (File.Exists(localPath))
			{
				LogService.Info("[Thumb] " + productId + " already cached");
				return localPath;
			}
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, imageUrl)
			{
				Headers = { { "Cookie", "age_check_done=1" } }
			};
			byte[] data = await (await _client.SendAsync(request)).Content.ReadAsByteArrayAsync();
			await File.WriteAllBytesAsync(localPath, data);
			LogService.Info($"[Thumb] {productId} downloaded ({data.Length} bytes)");
			return localPath;
		}
		catch
		{
			return null;
		}
	}

	private static string? GetJsonString(JsonElement element, string property)
	{
		JsonElement value;
		return (element.TryGetProperty(property, out value) && value.ValueKind == JsonValueKind.String) ? value.GetString() : null;
	}
}
