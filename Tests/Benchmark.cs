using System;
using System.Collections;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.TestTools;
using UnityTextureLoader.Cache;
using UnityTextureLoader.Extensions;

namespace UnityTextureLoader
{
	// Using Unity Perfomance testing
	// https://docs.unity3d.com/Packages/com.unity.test-framework.performance@2.8/manual/index.html
	public class Benchmark
	{
		private string testCacheRoot = "testCacheRoot";

		// large image 1.9 MB, 1600*1000
		private string urlRaw = "https://drive.google.com/uc?export=download&id=1eEFwCBD0feKO0fdm5E1bYrsS9pi_qs4q";

		[SetUp]
		[UnitySetUp]
		public void SetupTests()
		{
		}

		[TearDown]
		[UnityTearDown]
		public void ShutDown()
		{
		}

		[UnityTest]
		[Order(1)]
		[TestCase(typeof(MrtkDiskCacheProvider), ExpectedResult = null)]
		[TestCase(typeof(PlayerPrefsB64StringCache), ExpectedResult = null)]
		public IEnumerator SetupForBenchmark(System.Type typeOfCache)
		{
			AbstractDiscCache diskCache = System.Activator.CreateInstance(typeOfCache) as AbstractDiscCache;
			return UniTask.ToCoroutine(async () =>
			{
				diskCache.SetInitialCachePath(testCacheRoot);
				diskCache.RemoveCacheFolder();

				var loadTextureAsync = new LoadTextureAsync();
				loadTextureAsync.SetDiskLoader(diskCache);

				var urlNoToken = UrlExtensions.RemoveAllTokens(urlRaw);

				var texture =
					await loadTextureAsync.LoadTexture(Texture2D.blackTexture, urlRaw);
				Assert.IsTrue(texture != null);
				Assert.IsTrue(texture != Texture2D.blackTexture);
				texture.SafeDestroy();
				Assert.IsTrue(diskCache.FileExists(urlNoToken));

				var bytes = diskCache.Get(urlNoToken);
				Assert.IsTrue(bytes != null);
			});
		}

		[Order(2)]
		[Test]
		[NUnit.Framework.Category("DISK_CACHE")]
		[Performance]
		public void BenchMark_DiskCacheRetrieval_MRTK()
		{
			MeasureDiskCache<MrtkDiskCacheProvider>();
		}

		[Order(3)]
		[Test]
		[NUnit.Framework.Category("DISK_CACHE")]
		[Performance]
		public void BenchMark_DiskCacheRetrieval_PlayerPrefs()
		{
			MeasureDiskCache<PlayerPrefsB64StringCache>();
		}


		private void MeasureDiskCache<T>()
			where T : AbstractDiscCache, new()
		{
			AbstractDiscCache diskCache = new T();
			diskCache.SetInitialCachePath(testCacheRoot);
			var urlNoToken = UrlExtensions.RemoveAllTokens(urlRaw);
			var optimizedMultiply = 1 / 1048576f;


			Measure.Method(() => { _ = diskCache.Get(urlNoToken); })
				.WarmupCount(5)
				.MeasurementCount(100)
				.GC()
				.Run();
		}

		[Order(4)]
		[NUnit.Framework.Category("LoadingTexture")]
		[UnityTest]
		[Performance]
		public IEnumerator BenchMark_TextureCreating_MRTK()
		{
			return UniTask.ToCoroutine(MeasureLoadingTextureFull<MrtkDiskCacheProvider>);
		}

		[Order(5)]
		[NUnit.Framework.Category("LoadingTexture")]
		[UnityTest]
		[Performance]
		public IEnumerator BenchMark_TextureCreating_PlayerPrefs()
		{
			return UniTask.ToCoroutine(MeasureLoadingTextureFull<PlayerPrefsB64StringCache>);
		}

		private async UniTask MeasureLoadingTextureFull<T>()
			where T : AbstractDiscCache, new()
		{
			AbstractDiscCache diskCache = new T();
			diskCache.SetInitialCachePath(testCacheRoot);
			var loadTextureAsync = new LoadTextureAsync();
			loadTextureAsync.SetDiskLoader(diskCache);

			Texture2D texture = null;
			// Warmup
			for (int i = 0; i < 5; i++)
			{
				texture = await loadTextureAsync.LoadTexture(Texture2D.blackTexture, urlRaw);
				texture.SafeDestroy();
			}

			for (int i = 0; i < GC.MaxGeneration; i++)
			{
				GC.Collect(i, GCCollectionMode.Forced);
			}

			GC.Collect();
			await UniTask.Yield();

			var gc = new SampleGroup("GC", SampleUnit.Megabyte);
			var maxUsed = new SampleGroup("MaxUsedMemory", SampleUnit.Megabyte);
			var textureMemory = new SampleGroup("Texture", SampleUnit.Megabyte);
			var toConvert = 1024 * 1024;

			using (Measure.ProfilerMarkers(gc, maxUsed, textureMemory))
			{
				for (int i = 0; i < 50; i++)
				{
					GC.Collect();
					await UniTask.Yield();
					Profiler.BeginSample(gc.Name);
					using (Measure.Scope())
					{
						texture = await loadTextureAsync.LoadTexture(Texture2D.blackTexture, urlRaw);
						var usedHeadp = Profiler.usedHeapSizeLong;
						var maxUsedMem = Profiler.GetMonoUsedSizeLong();
						var unused = Profiler.GetRuntimeMemorySizeLong(texture);
						Profiler.EndSample();
						Measure.Custom(gc, usedHeadp / toConvert);
						Measure.Custom(maxUsed, maxUsedMem / toConvert);
						Measure.Custom(textureMemory, unused / toConvert);
					}

					texture.SafeDestroy();
				}
			}
		}
	}
}