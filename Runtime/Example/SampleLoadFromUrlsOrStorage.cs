using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityImageLoader.Cache;

namespace UnityImageLoader
{
	public class SampleLoadFromUrlsOrStorage : MonoBehaviour
	{
		// tips and tricks google drive : https://www.labnol.org/internet/direct-links-for-google-drive/28356/
		// google drive https://drive.google.com/uc?export=download&id=1GDbwdE3HVIqjQbNB9MpZFNhQgpBMWagW
		// google drive https://docs.google.com/document/d/DOC_FILE_ID/export?format=doc
		// amazon https://s3.amazonaws.com/omeka-net/59415/archive/files/519627986585e21157376c430ab085d6.png?AWSAccessKeyId=AKIAI3ATG3OSQLO5HGKA&Expires=1625097600&Signature=ZTHZlYJlXmJqLR4hyiU6sT0TtWQ%3D
		[SerializeField] private List<string> urls = new List<string>();
		[SerializeField] private bool LoadPack = false;
		[SerializeField] private bool ClearCache = false;
		[SerializeField] private List<RawImage> arrayOfImages = new List<RawImage>();

		[NonSerialized] private LoadImagesAsync _currentLoader = null;
		private CancellationTokenSource Source = null;

		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (LoadPack)
			{
				LoadPack = false;
				ClearImages();
				LoadAllImages().Forget();
			}

			if (ClearCache)
			{
				ClearCache = false;
				ClearImages();
			}
		}

		private void OnDisable()
		{
			Source?.Dispose();
			Source = new CancellationTokenSource();
		}

		private void ClearImages()
		{
			foreach (var rawImage in arrayOfImages)
			{
				rawImage.texture.SafeDestroy();
			}
		}

		private async UniTaskVoid LoadAllImages()
		{
			if (_currentLoader == null)
			{
				_currentLoader = new LoadImagesAsync();
				var loader = new MrtkDiskCacheProvider();
				loader.SetInitialCachePath("testLoadAsyncFolder");
				_currentLoader.SetDiskLoader(loader);
			}

			Source = new CancellationTokenSource();
			for (int i = 0; i < urls.Count; i++)
			{
				var texture = await _currentLoader.LoadImageAsync(Texture2D.redTexture, urls[i], null, Source.Token);
				await UniTask.SwitchToMainThread();
				arrayOfImages[i].texture = texture;
			}
		}
	}
}