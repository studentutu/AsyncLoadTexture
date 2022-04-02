using System;
using UnityEngine;
using UnityTextureLoader.Cache;

namespace UnityTextureLoader
{
	public class PlayerPrefsB64StringCache : AbstractDiscCache
	{
		private string _cacheKey = "cacheKey";

		public override bool SupportMultiThread()
		{
			return false;
		}

		public override void SetInitialCachePath(string localPath)
		{
			_cacheKey = localPath;
		}

		public override string GetCacheFolder()
		{
			return _cacheKey;
		}

		public override string GetPath(string url)
		{
			return null;
		}

		public override byte[] Get(string url)
		{
			var getuniqueString = GetUniqueHashFrom(url);
			var b64String = PlayerPrefs.GetString(getuniqueString);
			byte[] bytes = Convert.FromBase64String(b64String);
			return bytes;
		}

		public override void Set(string url, byte[] data)
		{
			var getuniqueString = GetUniqueHashFrom(url);
			var toB64 = Convert.ToBase64String(data);
			PlayerPrefs.SetString(getuniqueString, toB64);
			PlayerPrefs.Save();
		}

		public override bool FileExists(string urlOrPath)
		{
			var getuniqueString = GetUniqueHashFrom(urlOrPath);
			bool exists = PlayerPrefs.HasKey(getuniqueString);
			return exists;
		}

		public override void RemoveCacheFolder()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
		}


		public override void Dispose()
		{
		}
	}
}