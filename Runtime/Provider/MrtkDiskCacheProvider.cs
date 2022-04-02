using System.IO;
using UnityEngine;

namespace UnityTextureLoader.Cache
{
	/// <summary>
	/// Optional Setup by calling SetInitialCachePath("customPath");
	/// This cache works with Application.persistentDataPath - it has limitations for WebGL,PS,Nintendo.
	/// </summary>
	public class MrtkDiskCacheProvider : AbstractDiscCache
	{
		private string _cachePath = Path.Combine(Application.persistentDataPath, "customLocalRoot");

		public override void SetInitialCachePath(string localPath)
		{
			_cachePath = Path.Combine(Application.persistentDataPath, localPath);
		}

		public override string GetCacheFolder()
		{
			return _cachePath;
		}

		public override string GetPath(string url)
		{
			var uniqueHash = GetUniqueHashFrom(url);
			var pathUri = new System.Uri(FileExtensions.GetFilePathAsUrl(_cachePath));
			var uri = new System.Uri(Path.Combine(pathUri.AbsolutePath, "image" + uniqueHash));
			return uri.AbsoluteUri;
		}


		public override void Dispose()
		{
		}
	}
}