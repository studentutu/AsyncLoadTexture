using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class TextureExtensions
{
	/// <summary>
	/// Be sure to check if the file is present.
	/// </summary>
	/// <param name="fileUri">should start with file://</param>
	/// <param name="token"></param>
	/// <returns></returns>
	public static async UniTask<Texture2D> LoadTextureAsync(string fileUri, CancellationToken token)
	{
		await UniTask.SwitchToMainThread();
		var request = UnityWebRequestTexture.GetTexture(fileUri, true);
		await request.SendWebRequest();
		//texture loaded
		var texture = DownloadHandlerTexture.GetContent(request);
		return texture;
	}

	public static void SafeDestroy(this Texture createdTexture)
	{
		if (createdTexture == null)
		{
			return;
		}

		if (createdTexture == Texture2D.blackTexture)
		{
			return;
		}

		if (createdTexture == Texture2D.grayTexture)
		{
			return;
		}

		if (createdTexture == Texture2D.redTexture)
		{
			return;
		}

		if (createdTexture == Texture2D.linearGrayTexture)
		{
			return;
		}

		if (createdTexture == Texture2D.normalTexture)
		{
			return;
		}

		if (createdTexture == Texture2D.whiteTexture)
		{
			return;
		}

		bool useImmidiate = false;
#if UNITY_EDITOR
		useImmidiate = true;
#endif
		if (useImmidiate)
		{
			GameObject.DestroyImmediate(createdTexture);
		}
		else
		{
			GameObject.Destroy(createdTexture);
		}
	}
}