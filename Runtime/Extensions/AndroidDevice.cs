using System;
using UnityEngine;

namespace UnityTextureLoader.Extensions
{
	public static class AndroidDevice
	{
		private static bool IsAndroidRuntime()
		{
			bool result = false;
#if UNITY_ANDROID
			result = true;
#endif
#if UNITY_EDITOR
			result = false;
#endif
			return result;
		}

#if UNITY_ANDROID
		static AndroidJavaClass GetEnvironmentClass()
		{
			return new AndroidJavaClass("android.os.Environment");
		}

		static AndroidJavaObject GetActivity()
		{
			using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				return unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
		}
#endif

		public static long GetSdCardAvailableBytes()
		{
			string path = GetSDCardPath();
			if (!IsAndroidRuntime())
			{
				return GetMaxMemory();
			}
#if UNITY_ANDROID
			using (AndroidJavaObject statFs = new AndroidJavaObject("android.os.StatFs", path))
			{
				long size;
				if (GetBuildVersionSDKInt() >= 18)
				{
					size = statFs.Call<long>("getAvailableBlocksLong") * statFs.Call<long>("getBlockSizeLong");
				}
				else
				{
					size = statFs.Call<long>("getAvailableBlocks") * statFs.Call<long>("getBlockSize");
				}

				return size;
			}
#endif
		}

		public static string GetSDCardPath()
		{
			if (!IsAndroidRuntime())
			{
				return GetExternalCacheDir();
			}
#if UNITY_ANDROID
			using (AndroidJavaClass environment = GetEnvironmentClass())
			{
				using (AndroidJavaObject directory =
				       environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
				{
					return directory.Call<string>("getPath");
				}
			}
#endif
		}

		public static int GetBuildVersionSDKInt()
		{
			if (!IsAndroidRuntime())
			{
				return 0;
			}
#if UNITY_ANDROID
			using (AndroidJavaClass buildVersionClass = new AndroidJavaClass("android.os.Build$VERSION"))
			{
				return buildVersionClass.GetStatic<int>("SDK_INT");
			}
#endif
		}

		public static long GetMaxMemory()
		{
			if (!IsAndroidRuntime())
			{
				return SystemInfo.systemMemorySize;
			}
#if UNITY_ANDROID
			using (AndroidJavaClass runtime = new AndroidJavaClass("java.lang.Runtime"))
			{
				using (AndroidJavaObject run = runtime.CallStatic<AndroidJavaObject>("getRuntime"))
				{
					long maxMemory = run.Call<long>("maxMemory");
					return maxMemory;
				}
			}
#endif
		}

		public static bool IsExistSDCard()
		{
			if (!IsAndroidRuntime())
			{
				return false;
			}
#if UNITY_ANDROID
			using (AndroidJavaClass environment = GetEnvironmentClass())
			{
				string state = environment.CallStatic<string>("getExternalStorageState");
				return "mounted".Equals(state);
			}
#endif
		}

		public static string GetExternalCacheDir()
		{
			if (!IsAndroidRuntime())
			{
				return Application.persistentDataPath;
			}
#if UNITY_ANDROID
			using (AndroidJavaObject activity = GetActivity())
			{
				using (AndroidJavaObject cacheDir = activity.Call<AndroidJavaObject>("getExternalCacheDir"))
				{
					string path = cacheDir.Call<string>("getPath");
					return path;
				}
			}
#endif
		}
	}
}