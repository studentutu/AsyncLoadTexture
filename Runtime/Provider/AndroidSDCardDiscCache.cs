using System.IO;
using System;
using System.Collections;
using UnityTextureLoader.Extensions;

namespace UnityTextureLoader.Cache
{
	public class AndroidSDCardDiscCache : AbstractDiscCache
	{
		private string _cachePath = null;

		public AndroidSDCardDiscCache()
		{
			SetInitialCachePath(AndroidDevice.GetExternalCacheDir() + "/image_cache/");
		}

		public override void SetInitialCachePath(string path)
		{
			_cachePath = path;
		}

		public override void Set(string url, byte[] data)
		{
			if (data == null || data.Length <= 0)
			{
				return;
			}

			EnsureRootDirectory(_cachePath);

			string path = GetPath(url);

			if (!File.Exists(path))
			{
				long avaliableBytes = AndroidDevice.GetSdCardAvailableBytes();

				if (data.Length > avaliableBytes)
				{
					RemoveCacheFolder();
					EnsureRootDirectory(_cachePath);

					if (data.Length <= avaliableBytes)
					{
						File.WriteAllBytes(path, data);
					}
				}
				else
				{
					File.WriteAllBytes(path, data);
				}
			}
			else
			{
				File.SetLastAccessTime(path, DateTime.Now);
			}
		}

		public override byte[] Get(string url)
		{
			string path = GetPath(url);
			if (File.Exists(path))
			{
				byte[] data = File.ReadAllBytes(path);
				File.SetLastAccessTime(path, DateTime.Now);
				return data;
			}

			return null;
		}

		public override string GetPath(string url)
		{
			return Path.Combine(_cachePath, GetUniqueHashFrom(url));
		}

		public override void Dispose()
		{
		}

		public override string GetCacheFolder()
		{
			return _cachePath;
		}
	}
}