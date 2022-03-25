using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityImageLoader.Cache;

namespace UnityImageLoader
{
	public class ImageLoaderAsyncTest
	{
		private string testCacheRoot = "testCacheRoot";

		[SetUp]
		[UnitySetUp]
		public void SetupTests()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
		}

		[TearDown]
		[UnityTearDown]
		public void ShutDown()
		{
		}

		[UnityTest]
		[Order(1)]
		public IEnumerator LoadImagesAsync_ShouldLoadBytesFromUrl()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var url = "https://drive.google.com/uc?export=download&id=1GDbwdE3HVIqjQbNB9MpZFNhQgpBMWagW";
				var testClass = new LoadImagesAsync();

				var bytes = await testClass.LoadBytesViaUrl(url, null);
				Assert.IsNotNull(bytes);
			});
		}

		[UnityTest]
		public IEnumerator LoadImagesAsync_LoadImage()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var diskCache = new MrtkDiskCacheProvider();
				diskCache.SetInitialCachePath(testCacheRoot);
				diskCache.RemoveCacheFolder();

				var LoadImagesAsync = new LoadImagesAsync();
				LoadImagesAsync.SetDiskLoader(diskCache);

				var url = "https://drive.google.com/uc?export=download&id=1GDbwdE3HVIqjQbNB9MpZFNhQgpBMWagW";
				var path = diskCache.GetPath(url);
				Assert.IsNotNull(path);
				Assert.IsTrue(path.Length > 0);

				var texture =
					await LoadImagesAsync.LoadImageAsync(Texture2D.blackTexture, url, null, CancellationToken.None);
				Assert.IsTrue(texture != null);
				Assert.IsTrue(texture != Texture2D.blackTexture);
				texture.SafeDestroy();
				Assert.IsTrue(diskCache.FileExists(url));

				var bytes = diskCache.Get(url);
				Assert.IsTrue(bytes != null);
				diskCache.RemoveCacheFolder();
			});
		}

		[UnityTest]
		public IEnumerator DiskCacheShouldStoreAndRetrieve()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var diskCache = new MrtkDiskCacheProvider();
				diskCache.SetInitialCachePath(testCacheRoot);

				var someText =
					"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";

				var url = "https://drive.google.com/uc?export=download&id=1GDbwdE3HVIqjQbNB9MpZFNhQgpBMWagW";
				var path = diskCache.GetPath(url);
				Assert.IsNotNull(path);
				Assert.IsTrue(path.Length > 0);

				var bytes = Convert.FromBase64String(someText);
				Assert.IsNotNull(bytes);

				diskCache.Set(url, bytes);
				await UniTask.Yield();
				Assert.IsTrue(diskCache.FileExists(url));

				diskCache.RemoveCacheFolder();
			});
		}


		[Test]
		public void Url_ShouldBeTransformedIntoUriPath()
		{
			var diskCache = new MrtkDiskCacheProvider();
			diskCache.SetInitialCachePath(testCacheRoot);

			var testUrls =
				"https://s3.amazonaws.com/omeka-net/59415/archive/files/519627986585e21157376c430ab085d6.png?AWSAccessKeyId=AKIAI3ATG3OSQLO5HGKA&Expires=1625097600&Signature=ZTHZlYJlXmJqLR4hyiU6sT0TtWQ%3D";

			var path = diskCache.GetPath(testUrls);

			Assert.IsNotNull(path);
			Assert.IsTrue(path.Length > 0);
			Assert.IsTrue(path.StartsWith("file://"));

			testUrls = "https://drive.google.com/uc?export=download&id=1GDbwdE3HVIqjQbNB9MpZFNhQgpBMWagW";
			path = diskCache.GetPath(testUrls);
			Assert.IsNotNull(path);
			Assert.IsTrue(path.Length > 0);
			Assert.IsTrue(path.StartsWith("file://"));
		}
	}
}