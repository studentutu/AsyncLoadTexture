using System;
using System.IO;
using UnityEngine;

namespace UnityImageLoader.Cache
{
	/// <summary>
	/// Optional Setup by calling SetInitialCachePath("customPath");
	/// This cache works with Application.persistentDataPath - it has limitations for WebGL,PS,Nintendo.
	/// </summary>
	public class MrtkDiskCacheProvider : AbstractDiscCache
	{
		private string _cachePath = Path.Combine(Application.persistentDataPath, "customAppRoot");


		public override string GetPath(string url)
		{
			var uniqueHash = GetUniqueHashFrom(url);
			var pathUri = new System.Uri(FileExtensions.GetFilePathAsUrl(_cachePath));
			var absolutePath = Path.Combine(pathUri.AbsolutePath, "image" + uniqueHash);
			var uri = new System.Uri(absolutePath);
			EnsureRootDirectory(_cachePath);
			return uri.AbsoluteUri;
		}

		public override void Dispose()
		{
		}

		public override string GetCacheFolder()
		{
			return _cachePath;
		}

		public override void SetInitialCachePath(string path)
		{
			_cachePath = Path.Combine(Application.persistentDataPath, path);
		}

		public override void Set(string url, byte[] data)
		{
			EnsureRootDirectory(_cachePath);
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


		public override byte[] Get(string url)
		{
			var getPath = GetPath(url);
			bool mobile = File.Exists(getPath);
			if (mobile)
			{
				return GetInternalFile(getPath);
			}

			var uri = new System.Uri(getPath);
			return GetInternalFile(uri.AbsolutePath);
		}

		private byte[] GetInternalFile(string path)
		{
			byte[] bytes = null;
			if (File.Exists(path))
			{
				bytes = File.ReadAllBytes(path);
				File.SetLastAccessTime(path, DateTime.Now);
			}

			return bytes;
		}
	}
}