using System.IO;
using System;
using UnityEngine;

namespace UnityTextureLoader.Cache
{
	public abstract class AbstractDiscCache : IDisposable
	{
		public abstract void SetInitialCachePath(string path);
		public abstract string GetCacheFolder();

		/// <param name="url">Use Url with no tokens</param>
		public abstract string GetPath(string url);

		/// <param name="url">Use Url with no tokens</param>
		public virtual void Set(string url, byte[] data)
		{
			if (data == null || data.Length <= 0)
			{
				return;
			}

			EnsureRootDirectory(GetCacheFolder());
			var getPath = GetPath(url);
			bool mobileWrite = SafeWrite(getPath);
			if (mobileWrite)
			{
				File.WriteAllBytes(getPath, data);
				return;
			}

			var uri = new System.Uri(getPath);
			File.WriteAllBytes(uri.AbsolutePath, data);
		}

		private bool SafeWrite(string path)
		{
			try
			{
				var byteFakes = new byte[32];
				File.WriteAllBytes(path, byteFakes);
				return true;
			}
#pragma warning disable CS0168
			catch (Exception e)
#pragma warning restore CS0168
			{
				// ignored
			}

			return false;
		}

		/// <param name="url">Use Url with no tokens</param>
		public virtual byte[] Get(string url)
		{
			var uri = new System.Uri(GetPath(url));

			bool isExistsAsPath = File.Exists(uri.AbsolutePath);
			if (isExistsAsPath)
			{
				return GetInternal(uri.AbsolutePath);
			}

			bool isExistsAsUri = File.Exists(uri.AbsoluteUri);
			if (isExistsAsUri)
			{
				return GetInternal(uri.AbsoluteUri);
			}

			return null;
		}

		private byte[] GetInternal(string pathToUse)
		{
			if (File.Exists(pathToUse))
			{
				byte[] data = File.ReadAllBytes(pathToUse);
				File.SetLastAccessTime(pathToUse, DateTime.Now);
				return data;
			}

			return null;
		}

		/// <param name="urlOrPath">Use Url with no tokens, or Path</param>
		public virtual bool FileExists(string urlOrPath)
		{
			var uri = new System.Uri(GetPath(urlOrPath));
			var exists = File.Exists(uri.AbsoluteUri) || File.Exists(uri.AbsolutePath);
			return exists;
		}

		/// <summary>
		/// Transforms url with no tokens into unique hash string
		/// </summary>
		/// <param name="url">Use Url with no tokens</param>
		protected string GetUniqueHashFrom(string url)
		{
			var uri = new System.Uri(url);
			var afterHost = uri.PathAndQuery;
			var uniqueHash = Animator.StringToHash(System.Uri.EscapeUriString(afterHost));
			return uniqueHash.ToString();
		}

		/// <summary>
		/// Remove all cache entries recursively
		/// </summary>
		public void RemoveCacheFolder()
		{
			if (Directory.Exists(GetCacheFolder()))
			{
				Directory.Delete(GetCacheFolder(), true);
			}
		}

		/// <summary>
		/// Remove cache entries which are older than livespan
		/// </summary>
		public void RemoveOldCaches(string cachePath, TimeSpan liveSpan)
		{
			DirectoryInfo folder = new DirectoryInfo(cachePath);
			FileInfo[] files = folder.GetFiles();
			var dateTimeNow = DateTime.Now;
			var totalMinutes = liveSpan.TotalSeconds;
			for (int i = 0; i < files.Length; i++)
			{
				var differences = dateTimeNow - files[i].LastAccessTime;
				if (differences.TotalSeconds > totalMinutes)
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

		public abstract void Dispose();
	}
}