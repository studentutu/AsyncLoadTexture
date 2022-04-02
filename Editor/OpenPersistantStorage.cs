using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityTextureLoader.Cache;

public class OpenPersistantStorage
{
	[MenuItem("Tools/Open Persistant Storage")]
	public static void OnOpenPersistantStorage()
	{
		var diskCache = new MrtkDiskCacheProvider();
		var mockPath = "mockPath";
		diskCache.SetInitialCachePath(mockPath);
		var getPath = diskCache.GetCacheFolder();
		getPath = getPath.Substring(0, getPath.IndexOf(mockPath) - 1);
		Application.OpenURL(getPath);
	}
}