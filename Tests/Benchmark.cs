using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityTextureLoader
{
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