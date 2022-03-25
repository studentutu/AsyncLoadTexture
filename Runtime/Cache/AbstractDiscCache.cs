// hcq 2017/3/23

using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityImageLoader.Cache
{
	public abstract class AbstractDiscCache : IDisposable
	{
		private static Dictionary<string, Regex> _listRegexTokens = new Dictionary<string, Regex>()
		{
			{"token", new Regex(".+([Tt]oken)", RegexOptions.CultureInvariant | RegexOptions.Compiled)},
			{"expire", new Regex(".+([Ee]xpire)", RegexOptions.CultureInvariant | RegexOptions.Compiled)},
			{"access", new Regex(".+([Aa]ccess)", RegexOptions.CultureInvariant | RegexOptions.Compiled)},
		};

		public abstract void SetInitialCachePath(string path);
		public abstract void Set(string url, byte[] data);
		public abstract byte[] Get(string url);

		public virtual bool FileExists(string url)
		{
			string path = GetPath(url);
			var uri = new System.Uri(path);
			var exists = File.Exists(path) || File.Exists(uri.AbsolutePath);
			return exists;
		}

		public abstract string GetPath(string url);
		public abstract void Dispose();
		public abstract string GetCacheFolder();

		protected string GetUniqueHashFrom(string url)
		{
			var uri = new System.Uri(url);
			var afterHost = uri.PathAndQuery;
			afterHost = GetCachedKeyFromFullUrlWithToken(afterHost);
			afterHost = RemoveAllTokens(afterHost);
			var uniqueHash = Animator.StringToHash(System.Uri.EscapeUriString(afterHost));
			return uniqueHash.ToString();
		}

		public void RemoveCacheFolder()
		{
			if (Directory.Exists(GetCacheFolder()))
			{
				Directory.Delete(GetCacheFolder(), true);
			}
		}

		public void RemoveOldCaches(string cachePath, TimeSpan liveSpan)
		{
			DirectoryInfo folder = new DirectoryInfo(cachePath);
			FileInfo[] files = folder.GetFiles();
			var dateTimeNow = DateTime.Now;
			var totalMinutes = liveSpan.TotalMinutes;
			for (int i = 0; i < files.Length; i++)
			{
				var differences = files[i].LastAccessTime - dateTimeNow;
				if (differences.TotalMinutes > totalMinutes)
				{
					files[i].Delete();
				}
			}
		}

		public void EnsureRootDirectory(string cachePath)
		{
			if (!Directory.Exists(cachePath))
			{
				Directory.CreateDirectory(cachePath);
			}
		}

		/// <summary>
		/// Amazon specific filter. Trims Acess/expiration/signature
		/// </summary>
		/// <param name="fullUri"></param>
		/// <returns></returns>
		public static string GetCachedKeyFromFullUrlWithToken(string fullUri)
		{
			if (string.IsNullOrEmpty(fullUri))
			{
				return null;
			}

			var indexOf = fullUri.IndexOf("?AWS", StringComparison.InvariantCulture);
			if (indexOf < 0)
			{
				return fullUri;
			}

			return fullUri.Substring(0, indexOf);
		}

		public static string RemoveAllTokens(string fullUri)
		{
			if (string.IsNullOrEmpty(fullUri))
			{
				return fullUri;
			}

			var next = true;
			while (next)
			{
				next = false;
				foreach (var regexForTokens in _listRegexTokens)
				{
					var match = regexForTokens.Value.Match(fullUri);
					if (match.Success)
					{
						int currentlength = match.Value.Length;
						next = true;
						fullUri = match.Value.Substring(0, currentlength - regexForTokens.Key.Length);
					}
				}
			}

			return fullUri;
		}
	}

	public class FileDateSort : IComparer
	{
		public int Compare(object x, object y)
		{
			if (x == null && y == null)
			{
				return 0;
			}

			if (x == null)
			{
				return -1;
			}

			if (y == null)
			{
				return 1;
			}

			FileInfo xInfo = (FileInfo) x;
			FileInfo yInfo = (FileInfo) y;


			return xInfo.LastAccessTime.CompareTo(yInfo.LastAccessTime);
		}
	}
}