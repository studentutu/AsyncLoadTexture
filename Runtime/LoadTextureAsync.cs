using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityTextureLoader.Cache;
using UnityTextureLoader.Extensions;

namespace UnityTextureLoader
{
	/// <summary>
	/// Main class to load images. First setup _discCache. Later use LoadImageAsync
	/// </summary>
	public class LoadTextureAsync : IDisposable
	{
		private bool _disposed = false;
		private AbstractDiscCache _discCache = null;

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				DisposeFull();
			}
		}

		private void DisposeFull()
		{
			_discCache?.Dispose();
		}

		public LoadTextureAsync SetDiskLoader(AbstractDiscCache diskCache)
		{
			_discCache = diskCache;
			return this;
		}

		/// <summary>
		/// Will load fetch texture asynchronous. Using MRTK.Rest, File system to retrieve/store data.
		/// </summary>
		/// <param name="url">Raw url with all tokens</param>
		public async UniTask<Texture2D> LoadTexture(
			Texture2D errorTexture,
			string url)
		{
			return await LoadTexture(errorTexture, url, null, CancellationToken.None);
		}

		/// <summary>
		/// Will load fetch texture asynchronous. Using MRTK.Rest, File system to retrieve/store data.
		/// </summary>
		/// <param name="url">Raw url with all tokens</param>
		public async UniTask<Texture2D> LoadTexture(
			Texture2D errorTexture,
			string url,
			Dictionary<string, string> headers,
			CancellationToken token)
		{
			if (_discCache.SupportMultiThread())
			{
				await UniTask.SwitchToThreadPool();
			}
			else
			{
				await UniTask.SwitchToMainThread();
			}

			if (token.IsCancellationRequested)
			{
				return null;
			}

			var urlWithNoToken = UrlExtensions.RemoveAllTokens(url);
			var path = _discCache.GetPath(urlWithNoToken);
			System.Uri actualPath = null;
			if (!string.IsNullOrEmpty(path))
			{
				actualPath = new System.Uri(_discCache.GetPath(urlWithNoToken));
			}

			if (!_discCache.FileExists(urlWithNoToken))
			{
				var bytes = await LoadBytesViaUrl(url, headers, token);
				if (token.IsCancellationRequested)
				{
					return null;
				}

				if (bytes != null)
				{
					_discCache.Set(urlWithNoToken, bytes);
				}
			}

			Texture2D texture = null;
			if (actualPath != null)
			{
				texture = await TextureExtensions.LoadTextureAsync(actualPath.AbsoluteUri, token);
			}
			else
			{
				texture = await TextureExtensions.LoadTextureAsync(_discCache.Get(url), token);
			}


			if (texture == null)
			{
				texture = errorTexture;
			}

			return texture;
		}

		/// <summary>
		/// Will fetch and accept both base64 or byte[] data.
		/// </summary>
		/// <param name="url">Raw url with all tokens</param>
		public async UniTask<byte[]> LoadBytesViaUrl(
			string url,
			Dictionary<string, string> headers,
			CancellationToken token)
		{
			await UniTask.SwitchToMainThread();
			if (token.IsCancellationRequested)
			{
				return null;
			}

			var response = await Rest.GetAsync(url, headers, 20, null, true);
			if (token.IsCancellationRequested)
			{
				return null;
			}

			if (!response.Successful)
			{
#pragma warning disable CS0618
				Debug.LogError(response.ResponseCode + ":" + response.ResponseBody);
#pragma warning restore CS0618
				return null;
			}

			if (response.ResponseData != null)
			{
				return response.ResponseData;
			}

			var imageBase64 = await response.GetResponseBody();
			if (!string.IsNullOrEmpty(imageBase64))
			{
				return Convert.FromBase64String(imageBase64);
			}

			return null;
		}
	}
}