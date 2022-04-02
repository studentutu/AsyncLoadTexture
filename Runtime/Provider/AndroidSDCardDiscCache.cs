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

		public override bool SupportMultiThread()
		{
			return true;
		}

		public override void SetInitialCachePath(string localPath)
		{
			_cachePath = localPath;
		}

		public override string GetCacheFolder()
		{
			return _cachePath;
		}

		public override string GetPath(string url)
		{
			return Path.Combine(_cachePath, GetUniqueHashFrom(url));
		}

		public override void Set(string url, byte[] data)
		{
			if (data == null || data.Length <= 0)
			{
				return;
			}

			string path = GetPath(url);

			if (!File.Exists(path))
			{
				long avaliableBytes = AndroidDevice.GetSdCardAvailableBytes();

				if (data.Length > avaliableBytes)
				{
					RemoveCacheFolder();

					if (data.Length <= avaliableBytes)
					{
						base.Set(url, data);
					}
				}
				else
				{
					base.Set(url, data);
				}
			}
			else
			{
				File.SetLastAccessTime(path, DateTime.Now);
			}
		}

		public override void Dispose()
		{
		}
	}
}