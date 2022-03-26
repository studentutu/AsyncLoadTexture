using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityTextureLoader
{
	// TODO: Benchmark against Player Prefs and base 64
	public class Benchmark
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
	}
}